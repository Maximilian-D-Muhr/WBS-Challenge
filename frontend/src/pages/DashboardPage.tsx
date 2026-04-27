import { Link } from "react-router-dom";

import { useAuth } from "../contexts/AuthContext";
import { useChallenges } from "../data/challenges";
import ChallengeCard from "../components/ChallengeCard";

export default function DashboardPage() {
  const { user } = useAuth();
  const publicOpen = useChallenges({ visibility: "Public", status: "Open" });

  const myChallenges = publicOpen.data?.filter((c) => c.ownerId === user?.id) ?? [];
  const discover = publicOpen.data?.filter((c) => c.ownerId !== user?.id) ?? [];

  return (
    <section className="flex flex-col gap-10">
      <header className="flex items-center justify-between">
        <h1 className="text-3xl font-bold">Dashboard</h1>
        <Link to="/create" className="btn btn-primary btn-sm">
          New challenge
        </Link>
      </header>

      <section>
        <h2 className="text-xl font-semibold mb-3">My challenges</h2>
        {publicOpen.isLoading ? (
          <Loading />
        ) : myChallenges.length > 0 ? (
          <Grid>
            {myChallenges.map((c) => (
              <ChallengeCard key={c.id} challenge={c} />
            ))}
          </Grid>
        ) : (
          <p className="opacity-70">
            You haven't created any challenges yet.{" "}
            <Link to="/create" className="link link-primary">
              Start one?
            </Link>
          </p>
        )}
      </section>

      <section>
        <h2 className="text-xl font-semibold mb-3">Discover</h2>
        {publicOpen.isLoading ? (
          <Loading />
        ) : publicOpen.isError ? (
          <div className="alert alert-error">Could not load challenges.</div>
        ) : discover.length > 0 ? (
          <Grid>
            {discover.map((c) => (
              <ChallengeCard key={c.id} challenge={c} />
            ))}
          </Grid>
        ) : (
          <p className="opacity-70">No open public challenges yet.</p>
        )}
      </section>
    </section>
  );
}

function Loading() {
  return (
    <div className="flex justify-center py-6">
      <span className="loading loading-spinner loading-md" />
    </div>
  );
}

function Grid({ children }: { children: React.ReactNode }) {
  return <div className="grid sm:grid-cols-2 lg:grid-cols-3 gap-4">{children}</div>;
}
