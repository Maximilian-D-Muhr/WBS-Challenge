import { useLeaderboard } from "../data/leaderboards";

type Props = {
  challengeId: string;
  unitLabel: string;
};

// Top-N standings for a challenge. Total period only in phase 5.
export default function Leaderboard({ challengeId, unitLabel }: Props) {
  const { data, isLoading, isError } = useLeaderboard(challengeId, 10);

  if (isLoading) {
    return (
      <div className="flex justify-center py-4">
        <span className="loading loading-spinner loading-sm" />
      </div>
    );
  }

  if (isError || !data) {
    return <p className="opacity-70">Leaderboard unavailable.</p>;
  }

  if (data.entries.length === 0) {
    return <p className="opacity-70">No progress logged yet — be the first!</p>;
  }

  return (
    <div className="overflow-x-auto">
      <table className="table table-sm">
        <thead>
          <tr>
            <th className="w-12">#</th>
            <th>Member</th>
            <th className="text-right">Total ({unitLabel})</th>
            <th className="text-right">Days</th>
          </tr>
        </thead>
        <tbody>
          {data.entries.map((row) => (
            <tr key={row.userId}>
              <td className="font-mono">{row.rank}</td>
              <td>
                <div className="flex items-center gap-2">
                  <div className="avatar placeholder">
                    <div className="bg-neutral text-neutral-content w-7 rounded-full">
                      <span className="text-xs">
                        {row.displayName.slice(0, 2).toUpperCase()}
                      </span>
                    </div>
                  </div>
                  {row.displayName}
                </div>
              </td>
              <td className="text-right font-semibold">{row.totalAmount}</td>
              <td className="text-right opacity-70">{row.entryCount}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
