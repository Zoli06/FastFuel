import '@mantine/core/styles.css'
import './App.css'
import BrowseFoods from "./pages/BrowseFoods.tsx";

function App() {
    console.log(import.meta.env.VITE_GRAPHQL_HTTP_URL)
    return (
        <BrowseFoods />
    )
}

export default App
