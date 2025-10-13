import {gql, type TypedDocumentNode} from "@apollo/client";
import {useQuery} from "@apollo/client/react";
import type {FoodsQuery} from "../types/__generated__/graphql.ts";

const BROWSE_FOODS_QUERY: TypedDocumentNode<FoodsQuery> = gql`
    query Foods {
        foods {
            id
            name
            description
            imageUrl
        }
    }
`

const BrowseFoods = () => {
    const {data, loading, error} = useQuery(BROWSE_FOODS_QUERY);

    // TODO: Make better loading and error states
    if (loading) return <p>Loading...</p>;
    if (error) return <p>Error: {error.message}</p>;

    return (
        <div>
            {data!.foods.map(food => (
                <div key={food.id}>
                    <h2>{food.name}</h2>
                    <p>{food.description}</p>
                    <img src={food.imageUrl} alt={food.name} />
                </div>
            ))}
        </div>
    );
};

export default BrowseFoods;