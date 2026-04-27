import { useParams } from "react-router-dom";

import { useChallenge } from "../data/challenges";
import ChallengeStateBadge from "../components/ChallengeStateBadge";

export default function ChallengeDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { data: challenge, isLoading, isError } = useChallenge(id);

  if (isLoading) {
    return (
      <div className="flex justify-center py-10">
        <span className="loading loading-spinner loading-md" />
      </div>
    );
  }

  if (isError || !challenge) {
    return <div className="alert alert-error">Challenge not found.</div>;
  }

  return (
    <section className="max-w-2xl">
      <div className="flex items-center gap-3 mb-1">
        <h1 className="text-3xl font-bold">{challenge.title}</h1>
        <ChallengeStateBadge status={challenge.status} />
      </div>
      <p className="opacity-70 mb-6">by {challenge.ownerDisplayName}</p>

      {challenge.description && (
        <p className="mb-6 whitespace-pre-line">{challenge.description}</p>
      )}

      <dl className="grid grid-cols-[max-content_1fr] gap-x-6 gap-y-2 text-sm">
        <dt className="opacity-60">Starts</dt>
        <dd>{challenge.startDate}</dd>
        <dt className="opacity-60">Ends</dt>
        <dd>{challenge.endDate}</dd>
        <dt className="opacity-60">Unit</dt>
        <dd>{challenge.unitLabel}</dd>
        <dt className="opacity-60">Visibility</dt>
        <dd>{challenge.visibility}</dd>
      </dl>

      <p className="opacity-70 mt-8 text-sm">
        Join / leave / log progress arrive in phase 4.
      </p>
    </section>
  );
}
