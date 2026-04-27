import { toast } from "react-toastify";

import { useAuth } from "../contexts/AuthContext";
import { useJoinChallenge, useLeaveChallenge } from "../data/memberships";
import type { ApiError } from "../utils/problemDetails";
import type { MembershipStatus } from "../types/entities";

type Props = {
  challengeId: string;
  // The current user's membership for this challenge, if any.
  membership: { id: string; status: MembershipStatus } | null;
};

export default function JoinLeaveButton({ challengeId, membership }: Props) {
  const { token } = useAuth();
  const join = useJoinChallenge();
  const leave = useLeaveChallenge();

  if (!token) return null;

  const isMember = membership && membership.status !== "Left";

  async function handleJoin() {
    try {
      await join.mutateAsync(challengeId);
      toast.success("Joined challenge");
    } catch (err) {
      toast.error((err as ApiError).title);
    }
  }

  async function handleLeave() {
    if (!membership) return;
    try {
      await leave.mutateAsync({ membershipId: membership.id, challengeId });
      toast.info("Left challenge");
    } catch (err) {
      toast.error((err as ApiError).title);
    }
  }

  if (isMember) {
    return (
      <button
        type="button"
        className="btn btn-outline btn-sm"
        onClick={handleLeave}
        disabled={leave.isPending}
      >
        {leave.isPending ? "Leaving…" : "Leave"}
      </button>
    );
  }

  return (
    <button
      type="button"
      className="btn btn-primary btn-sm"
      onClick={handleJoin}
      disabled={join.isPending}
    >
      {join.isPending ? "Joining…" : "Join"}
    </button>
  );
}
