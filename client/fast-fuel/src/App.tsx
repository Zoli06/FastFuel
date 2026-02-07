import '@mantine/core/styles.css'
import './App.css'
import { Home } from './pages/Home.tsx';
import { Restaurant } from './components/Restaurant Menu/Restaurants.tsx';

function App() {
    return (
        <div className='app'> 
            <Restaurant />
        </div>
    )
}

export default App
