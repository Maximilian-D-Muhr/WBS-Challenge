// TanStack Query hooks for /progress-entries.
// The optimistic update pattern lives in the LogProgressForm component itself —
// the hook just exposes the mutation; cache surgery happens in the caller.

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { api } from "../utils/api";

// Wire shape for /progress-entries — mirrors ProgressEntryResponseDto.
export type ProgressEntryResponse = {
  id: string;
  userId: string;
  challengeId: string;
  amount: number;
  note?: string | null;
  loggedAt: string; // "YYYY-MM-DD"
  createdAt: string;
};

export type LogProgressPayload = {
  challengeId: string;
  amount: number;
  note?: string;
  loggedAt?: string; // "YYYY-MM-DD"; defaults to today on the server
};

export type UpdateProgressPayload = {
  amount: number;
  note?: string;
};

// GET /progress-entries/me/today?challengeId=...
// Backend returns 204 when no entry exists today — api wrapper turns that into undefined.
export function useMyTodayEntry(challengeId: string | undefined) {
  return useQuery({
    queryKey: ["progress", "today", challengeId],
    queryFn: () =>
      api<ProgressEntryResponse | undefined>(
        `/progress-entries/me/today?challengeId=${challengeId}`
      ),
    enabled: !!challengeId,
  });
}

// GET /progress-entries/me?challengeId=... — full personal history for this challenge.
export function useMyProgress(challengeId: string | undefined) {
  return useQuery({
    queryKey: ["progress", "mine", challengeId],
    queryFn: () =>
      api<ProgressEntryResponse[]>(`/progress-entries/me?challengeId=${challengeId}`),
    enabled: !!challengeId,
  });
}

export function useLogProgress(challengeId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (payload: LogProgressPayload) =>
      api<ProgressEntryResponse>("/progress-entries", { method: "POST", body: payload }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["progress", "today", challengeId] });
      qc.invalidateQueries({ queryKey: ["progress", "mine", challengeId] });
      qc.invalidateQueries({ queryKey: ["leaderboard", challengeId] });
    },
  });
}

export function useUpdateProgress(challengeId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: UpdateProgressPayload }) =>
      api<ProgressEntryResponse>(`/progress-entries/${id}`, { method: "PATCH", body: payload }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["progress", "today", challengeId] });
      qc.invalidateQueries({ queryKey: ["progress", "mine", challengeId] });
      qc.invalidateQueries({ queryKey: ["leaderboard", challengeId] });
    },
  });
}

export function useDeleteProgress(challengeId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) =>
      api<void>(`/progress-entries/${id}`, { method: "DELETE" }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["progress", "today", challengeId] });
      qc.invalidateQueries({ queryKey: ["progress", "mine", challengeId] });
      qc.invalidateQueries({ queryKey: ["leaderboard", challengeId] });
    },
  });
}
