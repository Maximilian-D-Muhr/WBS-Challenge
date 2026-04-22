import { useParams } from "react-router-dom";

export default function ChallengeDetailPage() {
  const { id } = useParams();

  return (
    <section>
      <h1 className="text-2xl font-bold mb-4">Challenge detail</h1>
      <p className="opacity-70">
        Showing challenge <code>{id}</code>. Full view arrives in phase 3.
      </p>
    </section>
  );
}
