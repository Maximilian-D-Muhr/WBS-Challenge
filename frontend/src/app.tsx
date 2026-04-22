import { createBrowserRouter, RouterProvider } from "react-router-dom";

import RootLayout from "./layouts/RootLayout";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import DashboardPage from "./pages/DashboardPage";
import ChallengeDetailPage from "./pages/ChallengeDetailPage";
import CreateChallengePage from "./pages/CreateChallengePage";

const router = createBrowserRouter([
  {
    path: "/",
    element: <RootLayout />,
    children: [
      { index: true, element: <DashboardPage /> },
      { path: "login", element: <LoginPage /> },
      { path: "register", element: <RegisterPage /> },
      { path: "create", element: <CreateChallengePage /> },
      { path: "challenges/:id", element: <ChallengeDetailPage /> },
    ],
  },
]);

export default function App() {
  return <RouterProvider router={router} />;
}
