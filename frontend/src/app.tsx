import { createBrowserRouter, RouterProvider } from "react-router-dom";

import RootLayout from "./layouts/RootLayout";
import ProtectedRoute from "./components/ProtectedRoute";
import ErrorBoundary from "./components/ErrorBoundary";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import DashboardPage from "./pages/DashboardPage";
import ChallengeDetailPage from "./pages/ChallengeDetailPage";
import CreateChallengePage from "./pages/CreateChallengePage";
import NotFoundPage from "./pages/NotFoundPage";

const router = createBrowserRouter([
  {
    path: "/",
    element: <RootLayout />,
    children: [
      {
        index: true,
        element: (
          <ProtectedRoute>
            <DashboardPage />
          </ProtectedRoute>
        ),
      },
      { path: "login", element: <LoginPage /> },
      { path: "register", element: <RegisterPage /> },
      {
        path: "create",
        element: (
          <ProtectedRoute>
            <CreateChallengePage />
          </ProtectedRoute>
        ),
      },
      {
        path: "challenges/:id",
        element: (
          <ProtectedRoute>
            <ChallengeDetailPage />
          </ProtectedRoute>
        ),
      },
      // Catch-all 404 — must be the last child of the root layout.
      { path: "*", element: <NotFoundPage /> },
    ],
  },
]);

export default function App() {
  return (
    <ErrorBoundary>
      <RouterProvider router={router} />
    </ErrorBoundary>
  );
}
