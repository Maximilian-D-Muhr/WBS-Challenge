// Thin fetch wrapper for the ChallengeTracker API.
// Token is injected by AuthContext via setAuthToken().

import { parseProblem, type ApiError } from "./problemDetails";

const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5290";

let currentToken: string | null = null;
export function setAuthToken(token: string | null) {
  currentToken = token;
}

type RequestOptions = {
  method?: "GET" | "POST" | "PATCH" | "PUT" | "DELETE";
  body?: unknown;
  headers?: Record<string, string>;
};

export async function api<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const { method = "GET", body, headers = {} } = options;

  const response = await fetch(`${BASE_URL}${path}`, {
    method,
    headers: {
      "Content-Type": "application/json",
      ...(currentToken ? { Authorization: `Bearer ${currentToken}` } : {}),
      ...headers,
    },
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!response.ok) {
    const problem = await parseProblem(response);
    const error: ApiError = Object.assign(new Error(problem.title), problem);
    throw error;
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}
