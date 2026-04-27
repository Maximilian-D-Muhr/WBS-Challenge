// TanStack Query mutations for /memberships.

import { useMutation, useQueryClient } from "@tanstack/react-query";

import { api } from "../utils/api";
import type { MembershipStatus } from "../types/entities";

export type MembershipResponse = {
  id: string;
  userId: string;
  challengeId: string;
  status: MembershipStatus;
  joinedAt: string;
};

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
    },
  });
}
