import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { z } from "zod";

import { useAuth } from "../contexts/AuthContext";
import { fieldErrors, type ApiError } from "../utils/problemDetails";

const schema = z.object({
  email: z.string().email("Enter a valid email"),
  password: z.string().min(8, "At least 8 characters").max(100),
  displayName: z.string().min(1, "Display name is required").max(50),
});

export default function RegisterPage() {
  const { register } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [displayName, setDisplayName] = useState("");
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [submitting, setSubmitting] = useState(false);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setErrors({});

    const parsed = schema.safeParse({ email, password, displayName });
    if (!parsed.success) {
      const flat = Object.fromEntries(
        parsed.error.issues.map((i) => [String(i.path[0]), i.message])
      );
      setErrors(flat);
      return;
    }

    setSubmitting(true);
    try {
      await register(email, password, displayName);
      navigate("/");
    } catch (err) {
      const apiErr = err as ApiError;
      setErrors({ form: apiErr.title, ...fieldErrors(apiErr) });
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <section className="max-w-sm mx-auto">
      <h1 className="text-2xl font-bold mb-4">Register</h1>

      <form onSubmit={handleSubmit} className="flex flex-col gap-3">
        <label className="form-control">
          <span className="label-text">Email</span>
          <input
            type="email"
            className="input input-bordered"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
          {errors.email && <span className="text-error text-sm mt-1">{errors.email}</span>}
        </label>

        <label className="form-control">
          <span className="label-text">Password</span>
          <input
            type="password"
            className="input input-bordered"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
          {errors.password && <span className="text-error text-sm mt-1">{errors.password}</span>}
        </label>

        <label className="form-control">
          <span className="label-text">Display name</span>
          <input
            type="text"
            className="input input-bordered"
            value={displayName}
            onChange={(e) => setDisplayName(e.target.value)}
          />
          {errors.displayName && (
            <span className="text-error text-sm mt-1">{errors.displayName}</span>
          )}
        </label>

        {errors.form && <div className="alert alert-error text-sm">{errors.form}</div>}

        <button type="submit" className="btn btn-primary" disabled={submitting}>
          {submitting ? "Creating account…" : "Create account"}
        </button>
      </form>
    </section>
  );
}
