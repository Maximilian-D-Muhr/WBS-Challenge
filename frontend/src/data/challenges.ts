// TanStack Query hooks for the Challenge endpoints.

import { useQuery } from "@tanstack/react-query";

import { api } from "../utils/api";
import type { ChallengeStatus, ChallengeVisibility } from "../types/entities";

// Wire shape for /challenges — matches ChallengeResponseDto on the backend.
export type ChallengeResponse = {
  id: string;
  ownerId: string;
  ownerDisplayName: string;
  title: string;
  description: string;
  visibility: ChallengeVisibility;
  status: ChallengeStatus;
  startDate: string; // "YYYY-MM-DD"
  endDate: string;
  unitLabel: string;
  createdAt: string;
};

type ListParams = {
  visibility?: ChallengeVisibility;
  status?: ChallengeStatus;
};

function buildQueryString(params: ListParams): string {
  const search = new URLSearchParams();
  if (params.visibility) search.set("visibility", params.visibility);
  if (params.status) search.set("status", params.status);
  const str = search.toString();
  return str ? `?${str}` : "";
}

export function useChallenges(params: ListParams = {}) {
  return useQuery({
    queryKey: ["challenges", params],
    queryFn: () => api<ChallengeResponse[]>(`/challenges${buildQueryString(params)}`),
  });
}

export function useChallenge(id: string | undefined) {
  return useQuery({
    queryKey: ["challenges", id],
    queryFn: () => api<ChallengeResponse>(`/challenges/${id}`),
    enabled: !!id,
  });
}
