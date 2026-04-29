import { toast } from "react-toastify";

import { useCompleteChallenge, useStartChallenge } from "../data/challenges";
import type { ApiError } from "../utils/problemDetails";
import type { ChallengeStatus } from "../types/entities";

type Props = {
  challengeId: string;
  status: ChallengeStatus;
};

export default function OwnerActions({ challengeId, status }: Props) {
  const start = useStartChallenge();
  const complete = useCompleteChallenge();

  async function handleStart() {
    try {
      await start.mutateAsync(challengeId);
      toast.success("Challenge started");
    } catch (err) {
      toast.error((err as ApiError).title);
    }
  }

  async function handleComplete() {
    try {
      await complete.mutateAsync(challengeId);
      toast.success("Challenge completed");
    } catch (err) {
      toast.error((err as ApiError).title);
    }
  }

  if (status === "Open") {
    return (
      <button
        type="button"
        className="btn btn-primary btn-sm"
        onClick={handleStart}
        disabled={start.isPending}
      >
        {start.isPending ? "Starting…" : "Start challenge"}
      </button>
    );
  }

  if (status === "Running") {
    return (
      <button
        type="button"
        className="btn btn-success btn-sm"
        onClick={handleComplete}
        disabled={complete.isPending}
      >
        {complete.isPending ? "Completing…" : "Complete challenge"}
      </button>
    );
  }

  return null;
}
