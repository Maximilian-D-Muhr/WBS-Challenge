import { useState } from "react";
import { toast } from "react-toastify";

import { useLogProgress } from "../data/progress";
import { fieldErrors, toastMessage, type ApiError } from "../utils/problemDetails";

type Props = {
  challengeId: string;
  unitLabel: string;
};

// Inline form on the detail page. Renders only when the user is an active
// member and hasn't logged for today yet — TodayProgress handles the rest.
export default function LogProgressForm({ challengeId, unitLabel }: Props) {
  const [amount, setAmount] = useState("");
  const [note, setNote] = useState("");
  const [errors, setErrors] = useState<Record<string, string>>({});

  const log = useLogProgress(challengeId);

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setErrors({});

    const parsed = Number(amount);
    if (!Number.isFinite(parsed) || parsed <= 0) {
      setErrors({ amount: "Amount must be a positive number." });
      return;
    }

    try {
      await log.mutateAsync({
        challengeId,
        amount: parsed,
        note: note.trim() || undefined,
      });
      setAmount("");
      setNote("");
      toast.success("Progress logged");
    } catch (err) {
      const apiErr = err as ApiError;
      const fields = fieldErrors(apiErr);
      if (Object.keys(fields).length) {
        setErrors(fields);
      } else if (apiErr.status === 409) {
        toast.error("You already logged for today.");
      } else {
        // Covers 429 (with Retry-After hint) and everything else.
        toast.error(toastMessage(apiErr));
      }
    }
  }

  return (
    <form onSubmit={handleSubmit} className="card bg-base-200 p-4 max-w-md">
      <h3 className="font-semibold mb-3">Log today's progress</h3>

      <label className="form-control mb-2">
        <span className="label-text">Amount ({unitLabel})</span>
        <input
          type="number"
          step="0.01"
          min="0.01"
          className="input input-bordered"
          value={amount}
          onChange={(e) => setAmount(e.target.value)}
          required
        />
        {errors.amount && (
          <span className="label-text-alt text-error mt-1">{errors.amount}</span>
        )}
      </label>

      <label className="form-control mb-3">
        <span className="label-text">Note (optional)</span>
        <input
          type="text"
          className="input input-bordered"
          value={note}
          onChange={(e) => setNote(e.target.value)}
          maxLength={500}
        />
        {errors.note && (
          <span className="label-text-alt text-error mt-1">{errors.note}</span>
        )}
      </label>

      <button type="submit" className="btn btn-primary btn-sm" disabled={log.isPending}>
        {log.isPending ? "Logging…" : "Log progress"}
      </button>
    </form>
  );
}
