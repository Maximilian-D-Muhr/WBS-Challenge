// Mirrors the C# models in backend/ChallengeTracker.Api/Models.
// Keep property names and enum values in sync — the API (de)serializes JSON with these exact keys.

export type ChallengeVisibility = "Public" | "Private";

export type ChallengeStatus = "Open" | "Running" | "Completed";

export type MembershipStatus = "Pending" | "Active" | "Left";

export type User = {
  id: string;
  displayName: string;
  email: string;
  createdAt: string; // ISO timestamp
};

export type Challenge = {
  id: string;
  ownerId: string;
  title: string;
  description: string;
  visibility: ChallengeVisibility;
  status: ChallengeStatus;
  startDate: string; // "YYYY-MM-DD"
  endDate: string;   // "YYYY-MM-DD"
  unitLabel: string;
  createdAt: string;
};

export type Membership = {
  id: string;
  userId: string;
  challengeId: string;
  status: MembershipStatus;
  joinedAt: string;
};

export type ProgressEntry = {
  id: string;
  userId: string;
  challengeId: string;
  amount: number;
  note?: string | null;
  loggedAt: string; // "YYYY-MM-DD"
  createdAt: string;
};
