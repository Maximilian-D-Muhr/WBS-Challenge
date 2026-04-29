// TanStack Query hook for /leaderboards/challenges/{id}.

import { useQuery } from "@tanstack/react-query";

import { api } from "../utils/api";

// Wire shape — mirrors LeaderboardResponseDto.
export type LeaderboardEntry = {
  rank: number;
  userId: string;
  displayName: string;
  totalAmount: number;
  entryCount: number;
};

export type LeaderboardResponse = {
  challengeId: string;
  period: string;
  entries: LeaderboardEntry[];
};

export function useLeaderboard(challengeId: string | undefined, top = 10) {
  return useQuery({
    queryKey: ["leaderboard", challengeId, top],
    queryFn: () =>
      api<LeaderboardResponse>(
        `/leaderboards/challenges/${challengeId}?period=total&top=${top}`
      ),
    enabled: !!challengeId,
  });
}
