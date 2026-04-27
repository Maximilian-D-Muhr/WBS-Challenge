import type { ChallengeStatus } from "../types/entities";

const VARIANT: Record<ChallengeStatus, string> = {
  Open: "badge-ghost",
  Running: "badge-primary",
  Completed: "badge-success",
};

export default function ChallengeStateBadge({ status }: { status: ChallengeStatus }) {
  return <span className={`badge ${VARIANT[status]}`}>{status}</span>;
}
