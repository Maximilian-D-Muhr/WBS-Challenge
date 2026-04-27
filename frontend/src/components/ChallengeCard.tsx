import { Link } from "react-router-dom";

import ChallengeStateBadge from "./ChallengeStateBadge";
import type { ChallengeResponse } from "../data/challenges";

export default function ChallengeCard({ challenge }: { challenge: ChallengeResponse }) {
  return (
    <Link
      to={`/challenges/${challenge.id}`}
      className="card bg-base-100 shadow-sm hover:shadow-md transition"
    >
      <div className="card-body">
        <div className="flex items-start justify-between gap-2">
          <h3 className="card-title text-lg leading-tight">{challenge.title}</h3>
          <ChallengeStateBadge status={challenge.status} />
        </div>
        {challenge.description && (
          <p className="text-sm opacity-70 line-clamp-2">{challenge.description}</p>
        )}
        <div className="text-xs opacity-60 mt-2">
          by {challenge.ownerDisplayName} · {challenge.unitLabel}
        </div>
      </div>
    </Link>
  );
}
