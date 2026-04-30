// RFC 7807 ProblemDetails parser.
// The backend returns either a plain ProblemDetails or a ValidationProblemDetails
// with a flat `errors` dictionary keyed by field name. 429 responses carry a
// Retry-After header that we surface to the UI so toasts can include the wait time.

export type ApiError = Error & {
  status: number;
  title: string;
  detail?: string;
  errors?: Record<string, string[]>;
  retryAfterSeconds?: number;
};

type RawProblem = {
  title?: string;
  detail?: string;
  status?: number;
  errors?: Record<string, string[]>;
};

export async function parseProblem(response: Response): Promise<Omit<ApiError, keyof Error>> {
  let raw: RawProblem = {};
  try {
    raw = (await response.json()) as RawProblem;
  } catch {
    // Response had no JSON body
  }

  // Retry-After can be either an integer (seconds) or an HTTP-date — the rate
  // limiter sets seconds, so we only handle that path here.
  let retryAfterSeconds: number | undefined;
  const retryHeader = response.headers.get("Retry-After");
  if (retryHeader) {
    const parsed = Number.parseInt(retryHeader, 10);
    if (Number.isFinite(parsed) && parsed > 0) retryAfterSeconds = parsed;
  }

  return {
    status: response.status,
    title: raw.title ?? defaultTitleFor(response.status),
    detail: raw.detail,
    errors: raw.errors,
    retryAfterSeconds,
  };
}

function defaultTitleFor(status: number): string {
  if (status === 429) return "Too many requests";
  if (status === 401) return "Not signed in";
  if (status === 403) return "Forbidden";
  if (status === 404) return "Not found";
  return `Request failed (${status})`;
}

// Helper for forms: returns a flat message per field, or a single toast message.
export function fieldErrors(error: ApiError): Record<string, string> {
  if (!error.errors) return {};
  return Object.fromEntries(
    Object.entries(error.errors).map(([key, msgs]) => [
      // ASP.NET capitalizes keys ("Email"); form libraries use camelCase — normalize.
      key.charAt(0).toLowerCase() + key.slice(1),
      msgs.join(" "),
    ])
  );
}

// Helper for toasts: turn an ApiError into a friendly one-liner.
// Falls back to the title for non-special-cased statuses.
export function toastMessage(error: ApiError): string {
  if (error.status === 429) {
    return error.retryAfterSeconds
      ? `Too many requests — try again in ${error.retryAfterSeconds}s.`
      : "Too many requests — slow down a moment.";
  }
  return error.title;
}
