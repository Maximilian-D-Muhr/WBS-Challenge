// TanStack Query mutations + queries for /memberships.

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { api } from "../utils/api";
import type { MembershipStatus } from "../types/entities";

export type MembershipResponse = {
  id: string;
  userId: string;
  challengeId: string;
  status: MembershipStatus;
  joinedAt: string;
};

// GET /memberships/me/{challengeId}
// Backend returns 204 if the caller has never joined — api wrapper turns that into undefined.
// JoinLeaveButton uses this to render the right action without a stale "Join" prompt.
export function useMyMembership(challengeId: string | undefined) {
  return useQuery({
    queryKey: ["memberships", "me", challengeId],
    queryFn: () =>
      api<MembershipResponse | undefined>(`/memberships/me/${challengeId}`),
    enabled: !!challengeId,
  });
}

export function useJoinChallenge() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (challengeId: string) =>
      api<MembershipResponse>("/memberships", {
        method: "POST",
        body: { challengeId },
      }),
    onSuccess: (_, challengeId) => {
      qc.invalidateQueries({ queryKey: ["challenges"] });
      qc.invalidateQueries({ queryKey: ["challenges", challengeId] });
      qc.invalidateQueries({ queryKey: ["memberships", "me", challengeId] });
    },
  });
}

export function useLeaveChallenge() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ membershipId }: { membershipId: string; challengeId: string }) =>
      api<void>(`/memberships/${membershipId}`, { method: "DELETE" }),
    onSuccess: (_, { challengeId }) => {
      qc.invalidateQueries({ queryKey: ["challenges"] });
      qc.invalidateQueries({ queryKey: ["challenges", challengeId] });
      qc.invalidateQueries({ queryKey: ["memberships", "me", challengeId] });
    },
  });
}
