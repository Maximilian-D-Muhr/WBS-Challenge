import { Link } from "react-router-dom";

export default function NotFoundPage() {
  return (
    <section className="hero py-16">
      <div className="hero-content text-center">
        <div className="max-w-md">
          <h1 className="text-7xl font-black opacity-30">404</h1>
          <h2 className="text-2xl font-bold mt-2 mb-3">Page not found</h2>
          <p className="opacity-80 mb-6">
            The page you were looking for doesn't exist or has moved.
          </p>
          <Link to="/" className="btn btn-primary">
            Back to dashboard
          </Link>
        </div>
      </div>
    </section>
  );
}
