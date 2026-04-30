import { Component, type ReactNode } from "react";

type Props = {
  children: ReactNode;
};

type State = {
  hasError: boolean;
};

// Top-level safety net for render-time crashes. React 19 still requires a class
// component for componentDidCatch — there's no hooks equivalent.
export default class ErrorBoundary extends Component<Props, State> {
  state: State = { hasError: false };

  static getDerivedStateFromError(): State {
    return { hasError: true };
  }

  componentDidCatch(error: Error, info: React.ErrorInfo) {
    // Console is fine for the demo. Replace with a real logger when we have one.
    console.error("ErrorBoundary caught:", error, info);
  }

  handleReload = () => {
    window.location.reload();
  };

  render() {
    if (!this.state.hasError) return this.props.children;

    return (
      <div className="min-h-screen flex items-center justify-center p-6">
        <div className="card bg-base-200 max-w-md text-center p-8">
          <h1 className="text-2xl font-bold mb-2">Something went wrong</h1>
          <p className="opacity-80 mb-6">
            The app hit an unexpected error. Reloading usually clears it.
          </p>
          <button type="button" className="btn btn-primary" onClick={this.handleReload}>
            Reload
          </button>
        </div>
      </div>
    );
  }
}
