public class Node
{
    public int Value;
    public Node Left, Right;
    public Node(int value)
    {
        Value = value;
        Left = Right = null;
    }
}
public class BinaryTree
{
    public Node Root;
    public void Insert(int value)
    {
        if (Root == null)
            Root = new Node(value);
        else
            InsertRecursive(Root, value);
    }
    private void InsertRecursive(Node current, int value)
    {
        if (value < current.Value)
        {
            if (current.Left == null)
                current.Left = new Node(value);
            else
                InsertRecursive(current.Left, value);
        }
        else
        {
            if (current.Right == null)
                current.Right = new Node(value);
            else
                InsertRecursive(current.Right, value);
        }
    }
    public void PrintInOrder(Node node)
    {
        if (node == null) return;
        PrintInOrder(node.Left);
        Console.Write(node.Value + " ");
        PrintInOrder(node.Right);
    }
}