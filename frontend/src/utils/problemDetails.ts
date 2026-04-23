// RFC 7807 ProblemDetails parser.
// The backend returns either a plain ProblemDetails or a ValidationProblemDetails
// with a flat `errors` dictionary keyed by field name.

export type ApiError = Error & {
  status: number;
  title: string;
  detail?: string;
  errors?: Record<string, string[]>;
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

  return {
    status: response.status,
    title: raw.title ?? `Request failed (${response.status})`,
    detail: raw.detail,
    errors: raw.errors,
  };
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
