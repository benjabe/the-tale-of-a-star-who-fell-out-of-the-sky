using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Node
{
    public Vector3 position;
    public Dictionary<Node, float> edges;

    public Node(Vector3 p)
    {
        position = p;
        edges = new Dictionary<Node, float>();
    }

    public void AddEdge(Node node)
    {
        edges.Add(node, Vector3.Distance(position, node.position));
        if (!node.edges.ContainsKey(this))
        {
            node.AddEdge(this);
        }
    }
}

public class FlyingEnemy : Enemy
{
    float timeBetweenPathSearch = 1.0f;
    float timeSincePathSearch = 1.0f;
    List<Node> graph = null;
    List<Node> pathToPlayer = null;
    Node homeNode = null;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        GetComponent<Rigidbody2D>().gravityScale = 0.0f;
        //GenerateGraph();
        //pathToPlayer = FindPathToPlayer();
    }
    private void Update()
    {
        timeSinceLastJump += Time.deltaTime;
        /*
        timeSincePathSearch += Time.deltaTime;
        // Find path to player
        if (timeSincePathSearch >= timeBetweenPathSearch)
        {
            timeSincePathSearch = 0.0f;
            FindPathToPlayer();
        }
        // Move along path
        if (pathToPlayer != null)
        {
            Node nextNode = pathToPlayer[pathToPlayer.Count - 1];
            Vector3 direction = nextNode.position - transform.position;
            float distance = direction.magnitude;
            direction.Normalize();
            float distanceToMove = speed * Time.deltaTime;
            if (distanceToMove >= distance)
            {
                transform.position = nextNode.position;
                pathToPlayer.Remove(nextNode);
                if (pathToPlayer.Count <= 0)
                {
                    pathToPlayer = null;
                }
            }
            else
            {
                transform.position += direction * distanceToMove;
            }
        }
        else
        {
            // Find path to home node
        }
        */
        if (Vector3.Distance(player.transform.position, transform.position) < maxAggressionDistance)
        {
            Vector3 distanceToMove = (player.transform.position - transform.position).normalized *
                speed * Time.deltaTime;

            if (timeSinceLastJump >= timeBetweenJumps)
            {
                GetComponent<Rigidbody2D>()
                    .AddForce(
                        (Random.Range(0,2) == 0 ? Vector2.up : Vector2.right)
                        * (Random.Range(0, 2) == 0 ? 2.0f : -2.0f),
                        ForceMode2D.Impulse
                    );
                timeSinceLastJump = 0.0f;
            }

            transform.position += distanceToMove;
        }
    }

    void GenerateGraph()
    {
        List<Node> nodeList = new List<Node>();
        float nearest = Mathf.Infinity;
        // Generate nodes
        for (float x = transform.position.x - 8.0f; x < transform.position.x + 8.0f; x += 0.5f)
        {
            for (float y = transform.position.y - 8.0f; y < transform.position.y + 8.0f; y += 0.5f)
            {
                Node newNode = new Node(new Vector3(x, y, 0.0f));
                float distance = Vector3.Distance(newNode.position, transform.position);
                if (distance < nearest)
                {
                    homeNode = newNode;
                    nearest = distance;
                }
                nodeList.Add(newNode);
                foreach (Node node in nodeList)
                {
                    if (Vector3.Distance(node.position, newNode.position) < 0.9f)
                    {
                        newNode.AddEdge(node);
                    }
                }
            }
        }
        graph = nodeList;
    }

    List<Node> FindPathToPlayer()
    {
        if (Vector3.Distance(player.transform.position, transform.position) > 15.0f)
        {
            return null;
        }
        Node startNode = null;
        Node goalNode = null;
        float nearest = Mathf.Infinity;
        float nearestToPlayer = Mathf.Infinity;

        foreach (Node node in graph)
        {
            float distance = Vector3.Distance(node.position, transform.position);
            if (distance < nearest)
            {
                startNode = node;
                nearest = distance;
            }
            float distanceToPlayer = Vector3.Distance(node.position, player.transform.position);
            if (distanceToPlayer < nearestToPlayer)
            {
                goalNode = node;
                nearestToPlayer = distanceToPlayer;
            }
        }

        // Find path to player
        List<Node> closedSet = new List<Node>();
        List<Node> openSet = new List<Node>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        openSet.Add(startNode);
        Dictionary<Node, float> gScore = new Dictionary<Node, float>();
        gScore[startNode] = 0.0f;
        Dictionary<Node, float> fScore = new Dictionary<Node, float>();
        fScore[startNode] = HeuristicCostEstimate(startNode, goalNode);

        List<Node> totalPath = null;

        while (openSet.Count > 0)
        {
            // Find node in openSet with lowest fScore
            Node current = null;
            foreach (Node node in openSet)
            {
                if (current == null || fScore[node] < fScore[current])
                {
                    current = node;
                }
            }

            if (current == goalNode)
            {
                totalPath = ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Node neighbour in current.edges.Keys)
            {
                if (closedSet.Contains(neighbour))
                {
                    continue;
                }
                float tentativeGScore = gScore[current] + current.edges[neighbour];

                if (!openSet.Contains(neighbour))
                {
                    openSet.Add(neighbour);
                }
                else if (tentativeGScore >= gScore[neighbour])
                {
                    continue;
                }

                cameFrom[neighbour] = current;
                gScore[neighbour] = tentativeGScore;
                fScore[neighbour] = gScore[neighbour] + HeuristicCostEstimate(neighbour, goalNode);
            }
        }
        return totalPath;
    }

    List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        List<Node> totalPath = new List<Node>();
        totalPath.Add(current);
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current);
        }
        return totalPath;
    }

    private float HeuristicCostEstimate(Node start, Node goal)
    {
        return Vector3.Distance(start.position, goal.position);
    }
}
