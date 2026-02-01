import '@mantine/core/styles.css'
import './App.css'
import BrowseFoods from "./pages/BrowseFoods.tsx";
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

const queryClient = new QueryClient();

function App() {
    return (
        <QueryClientProvider client={queryClient}>
            <BrowseFoods/>
        </QueryClientProvider>
    )
}

export default App
