import { toast } from "react-toastify";

import { useDeleteProgress, useMyTodayEntry } from "../data/progress";
import type { ApiError } from "../utils/problemDetails";
import LogProgressForm from "./LogProgressForm";

type Props = {
  challengeId: string;
  unitLabel: string;
};

// Shows either the LogProgressForm (no entry yet today) or a summary card
// with the existing entry + a delete button (within the 24h edit window).
export default function TodayProgress({ challengeId, unitLabel }: Props) {
  const { data: entry, isLoading } = useMyTodayEntry(challengeId);
  const del = useDeleteProgress(challengeId);

  if (isLoading) {
    return <span className="loading loading-dots loading-sm" />;
  }

  if (!entry) {
    return <LogProgressForm challengeId={challengeId} unitLabel={unitLabel} />;
  }

  async function handleDelete() {
    if (!entry) return;
    try {
      await del.mutateAsync(entry.id);
      toast.success("Entry removed");
    } catch (err) {
      toast.error((err as ApiError).title);
    }
  }

  return (
    <div className="card bg-base-200 p-4 max-w-md">
      <h3 className="font-semibold mb-2">Today's entry</h3>
      <p className="text-2xl font-bold">
        {entry.amount} <span className="text-base font-normal opacity-70">{unitLabel}</span>
      </p>
      {entry.note && <p className="opacity-80 mt-1">{entry.note}</p>}
      <button
        type="button"
        className="btn btn-ghost btn-xs self-start mt-3"
        onClick={handleDelete}
        disabled={del.isPending}
      >
        {del.isPending ? "Removing…" : "Remove (within 24h)"}
      </button>
    </div>
  );
}
