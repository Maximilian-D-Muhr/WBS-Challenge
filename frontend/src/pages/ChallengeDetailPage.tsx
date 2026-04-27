import { useParams } from "react-router-dom";

import { useAuth } from "../contexts/AuthContext";
import { useChallenge } from "../data/challenges";
import ChallengeStateBadge from "../components/ChallengeStateBadge";
import JoinLeaveButton from "../components/JoinLeaveButton";
import OwnerActions from "../components/OwnerActions";

export default function ChallengeDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { user } = useAuth();
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

  const isOwner = user?.id === challenge.ownerId;

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

      <dl className="grid grid-cols-[max-content_1fr] gap-x-6 gap-y-2 text-sm mb-6">
        <dt className="opacity-60">Starts</dt>
        <dd>{challenge.startDate}</dd>
        <dt className="opacity-60">Ends</dt>
        <dd>{challenge.endDate}</dd>
        <dt className="opacity-60">Unit</dt>
        <dd>{challenge.unitLabel}</dd>
        <dt className="opacity-60">Visibility</dt>
        <dd>{challenge.visibility}</dd>
      </dl>

      <div className="flex gap-2">
        {isOwner ? (
          <OwnerActions challengeId={challenge.id} status={challenge.status} />
        ) : (
          // Phase 4: we don't yet load the user's membership status from the detail endpoint,
          // so the Join button always shows. Phase 5 will introduce a memberships query
          // and render Leave when appropriate.
          <JoinLeaveButton challengeId={challenge.id} membership={null} />
        )}
      </div>

      <p className="opacity-70 mt-8 text-sm">
        Progress logging arrives in phase 5.
      </p>
    </section>
  );
}
