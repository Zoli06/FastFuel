import {gql, type TypedDocumentNode} from "@apollo/client";
import {useQuery} from "@apollo/client/react";
import type {FoodsQuery} from "../types/__generated__/graphql.ts";
import {Badge, Button, Card, Group, Image, Text} from "@mantine/core";

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
                <Card shadow="sm" padding="lg" radius="md" withBorder>
                    <Card.Section>
                        <Image
                            src={food.imageUrl}
                            height={160}
                            alt={food.name}
                        />
                    </Card.Section>

                    <Group justify="space-between" mt="md" mb="xs">
                        <Text fw={500}>Norway Fjord Adventures</Text>
                        <Badge color="pink">On Sale</Badge>
                    </Group>

                    <Text size="sm" c="dimmed">
                        With Fjord Tours you can explore more of the magical fjord landscapes with tours and
                        activities on and around the fjords of Norway
                    </Text>

                    <Button color="blue" fullWidth mt="md" radius="md">
                        Book classic tour now
                    </Button>
                </Card>
            ))}
        </div>
    );
};

export default BrowseFoods;