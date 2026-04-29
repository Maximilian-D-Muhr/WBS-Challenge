import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { z } from "zod";

import { useCreateChallenge } from "../data/challenges";
import { fieldErrors, type ApiError } from "../utils/problemDetails";

const schema = z
  .object({
    title: z.string().min(1, "Title is required").max(100),
    description: z.string().max(1000).optional(),
    visibility: z.enum(["Public", "Private"]),
    startDate: z.string().min(1, "Start date is required"),
    endDate: z.string().min(1, "End date is required"),
    unitLabel: z.string().min(1, "Unit is required").max(50),
  })
  .refine((v) => v.endDate >= v.startDate, {
    message: "End date must be on or after start date",
    path: ["endDate"],
  });

export default function CreateChallengePage() {
  const navigate = useNavigate();
  const create = useCreateChallenge();

  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [visibility, setVisibility] = useState<"Public" | "Private">("Public");
  const [startDate, setStartDate] = useState(today());
  const [endDate, setEndDate] = useState(today(7));
  const [unitLabel, setUnitLabel] = useState("");
  const [errors, setErrors] = useState<Record<string, string>>({});

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setErrors({});

    const parsed = schema.safeParse({ title, description, visibility, startDate, endDate, unitLabel });
    if (!parsed.success) {
      setErrors(Object.fromEntries(parsed.error.issues.map((i) => [String(i.path[0]), i.message])));
      return;
    }

    try {
      const created = await create.mutateAsync({
        title: parsed.data.title,
        description: parsed.data.description,
        visibility: parsed.data.visibility,
        startDate: parsed.data.startDate,
        endDate: parsed.data.endDate,
        unitLabel: parsed.data.unitLabel,
      });
      navigate(`/challenges/${created.id}`);
    } catch (err) {
      const apiErr = err as ApiError;
      setErrors({ form: apiErr.title, ...fieldErrors(apiErr) });
    }
  }

  return (
    <section className="max-w-md mx-auto">
      <h1 className="text-2xl font-bold mb-4">New challenge</h1>

      <form onSubmit={handleSubmit} className="flex flex-col gap-3">
        <Field label="Title" error={errors.title}>
          <input
            type="text"
            className="input input-bordered"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
          />
        </Field>

        <Field label="Description" error={errors.description}>
          <textarea
            className="textarea textarea-bordered"
            rows={3}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
        </Field>

        <Field label="Visibility" error={errors.visibility}>
          <select
            className="select select-bordered"
            value={visibility}
            onChange={(e) => setVisibility(e.target.value as "Public" | "Private")}
          >
            <option value="Public">Public</option>
            <option value="Private">Private</option>
          </select>
        </Field>

        <div className="grid grid-cols-2 gap-3">
          <Field label="Start date" error={errors.startDate}>
            <input
              type="date"
              className="input input-bordered"
              value={startDate}
              onChange={(e) => setStartDate(e.target.value)}
            />
          </Field>
          <Field label="End date" error={errors.endDate}>
            <input
              type="date"
              className="input input-bordered"
              value={endDate}
              onChange={(e) => setEndDate(e.target.value)}
            />
          </Field>
        </div>

        <Field label="Unit (e.g. pages, km, minutes)" error={errors.unitLabel}>
          <input
            type="text"
            className="input input-bordered"
            value={unitLabel}
            onChange={(e) => setUnitLabel(e.target.value)}
          />
        </Field>

        {errors.form && <div className="alert alert-error text-sm">{errors.form}</div>}

        <button type="submit" className="btn btn-primary" disabled={create.isPending}>
          {create.isPending ? "Creating…" : "Create challenge"}
        </button>
      </form>
    </section>
  );
}

function Field({ label, error, children }: { label: string; error?: string; children: React.ReactNode }) {
  return (
    <label className="form-control">
      <span className="label-text">{label}</span>
      {children}
      {error && <span className="text-error text-sm mt-1">{error}</span>}
    </label>
  );
}

function today(offsetDays: number = 0): string {
  const d = new Date();
  d.setDate(d.getDate() + offsetDays);
  return d.toISOString().slice(0, 10);
}
