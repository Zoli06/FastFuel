import {StrictMode} from 'react'
import {createRoot} from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import {ApolloClient, HttpLink, InMemoryCache} from "@apollo/client";
import {ApolloProvider} from "@apollo/client/react";
import {MantineProvider} from "@mantine/core";

const client = new ApolloClient({
    link: new HttpLink({uri: import.meta.env.VITE_GRAPHQL_HTTP_URL}),
    cache: new InMemoryCache(),
    dataMasking: true,
});

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <ApolloProvider client={client}>
            <MantineProvider>
                <App/>
            </MantineProvider>
        </ApolloProvider>
    </StrictMode>
)
