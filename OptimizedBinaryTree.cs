using System;

// Optimized Node class with height tracking for AVL balancing
public class OptimizedNode
{
    public int Value;           // Task priority value
    public OptimizedNode? Left, Right;    // Child nodes (nullable for proper initialization)
    public int Height;          // Height for AVL balancing - prevents tree degradation
    
    public OptimizedNode(int value)
    {
        Value = value;
        Left = Right = null;
        Height = 1; // Leaf nodes start with height 1
    }
}

// Optimized Binary Tree with AVL self-balancing for consistent O(log n) performance
public class OptimizedBinaryTree
{
    public OptimizedNode? Root;
    
    // LLM Optimization: Added efficient search functionality for task retrieval
    public bool Search(int value)
    {
        return SearchRecursive(Root, value);
    }
    
    private bool SearchRecursive(OptimizedNode? node, int value)
    {
        if (node == null) return false;
        if (node.Value == value) return true;
        
        // Binary search optimization - only traverse one side
        return value < node.Value ? 
            SearchRecursive(node.Left, value) : 
            SearchRecursive(node.Right, value);
    }
    
    // LLM Optimization: Self-balancing insert maintains O(log n) performance
    public void Insert(int value)
    {
        Root = InsertRecursive(Root, value);
    }
    
    private OptimizedNode InsertRecursive(OptimizedNode? node, int value)
    {
        // Standard BST insertion
        if (node == null)
            return new OptimizedNode(value);
            
        if (value < node.Value)
            node.Left = InsertRecursive(node.Left, value);
        else if (value > node.Value)
            node.Right = InsertRecursive(node.Right, value);
        else
            return node; // Duplicate values not allowed in task priorities
        
        // Update height of current node
        node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
        
        // Get balance factor to check if rebalancing is needed
        int balance = GetBalance(node);
        
        // Left Left Case - Right rotation needed
        if (balance > 1 && value < node.Left!.Value)
            return RightRotate(node);
        
        // Right Right Case - Left rotation needed
        if (balance < -1 && value > node.Right!.Value)
            return LeftRotate(node);
        
        // Left Right Case - Left rotation on left child, then right rotation
        if (balance > 1 && value > node.Left!.Value)
        {
            node.Left = LeftRotate(node.Left);
            return RightRotate(node);
        }
        
        // Right Left Case - Right rotation on right child, then left rotation
        if (balance < -1 && value < node.Right!.Value)
        {
            node.Right = RightRotate(node.Right);
            return LeftRotate(node);
        }
        
        return node; // No rotation needed
    }
    
    // LLM Optimization: Helper methods for AVL balancing
    private int GetHeight(OptimizedNode? node)
    {
        return node?.Height ?? 0;
    }
    
    private int GetBalance(OptimizedNode? node)
    {
        return node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);
    }
    
    // Right rotation for AVL balancing
    private OptimizedNode RightRotate(OptimizedNode y)
    {
        OptimizedNode x = y.Left!;
        OptimizedNode? T2 = x.Right;
        
        // Perform rotation
        x.Right = y;
        y.Left = T2;
        
        // Update heights
        y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;
        x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;
        
        return x; // New root
    }
    
    // Left rotation for AVL balancing
    private OptimizedNode LeftRotate(OptimizedNode x)
    {
        OptimizedNode y = x.Right!;
        OptimizedNode? T2 = y.Left;
        
        // Perform rotation
        y.Left = x;
        x.Right = T2;
        
        // Update heights
        x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;
        y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;
        
        return y; // New root
    }
    
    // LLM Optimization: Multiple traversal methods for different use cases
    public void PrintInOrder(OptimizedNode? node)
    {
        if (node == null) return;
        PrintInOrder(node.Left);
        Console.Write(node.Value + " ");
        PrintInOrder(node.Right);
    }
    
    // LLM Optimization: Find minimum priority task (leftmost node)
    public int FindMin()
    {
        if (Root == null) throw new InvalidOperationException("Tree is empty");
        return FindMinRecursive(Root).Value;
    }
    
    private OptimizedNode FindMinRecursive(OptimizedNode node)
    {
        while (node.Left != null)
            node = node.Left;
        return node;
    }
    
    // LLM Optimization: Find maximum priority task (rightmost node)
    public int FindMax()
    {
        if (Root == null) throw new InvalidOperationException("Tree is empty");
        return FindMaxRecursive(Root).Value;
    }
    
    private OptimizedNode FindMaxRecursive(OptimizedNode node)
    {
        while (node.Right != null)
            node = node.Right;
        return node;
    }
    
    // LLM Optimization: Delete operation for task completion
    public void Delete(int value)
    {
        Root = DeleteRecursive(Root, value);
    }
    
    private OptimizedNode? DeleteRecursive(OptimizedNode? node, int value)
    {
        if (node == null) return node;
        
        if (value < node.Value)
            node.Left = DeleteRecursive(node.Left, value);
        else if (value > node.Value)
            node.Right = DeleteRecursive(node.Right, value);
        else
        {
            // Node to be deleted found
            if (node.Left == null || node.Right == null)
            {
                OptimizedNode? temp = node.Left ?? node.Right;
                if (temp == null)
                {
                    node = null;
                }
                else
                {
                    // Copy the contents of non-empty child
                    node.Value = temp.Value;
                    node.Left = temp.Left;
                    node.Right = temp.Right;
                    node.Height = temp.Height;
                }
            }
            else
            {
                // Node with two children: Get inorder successor
                OptimizedNode temp = FindMinRecursive(node.Right);
                node.Value = temp.Value;
                node.Right = DeleteRecursive(node.Right, temp.Value);
            }
        }
        
        if (node == null) return node;
        
        // Update height and rebalance
        node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
        int balance = GetBalance(node);
        
        // Rebalance if needed
        if (balance > 1 && GetBalance(node.Left) >= 0)
            return RightRotate(node);
        if (balance > 1 && GetBalance(node.Left) < 0)
        {
            node.Left = LeftRotate(node.Left!);
            return RightRotate(node);
        }
        if (balance < -1 && GetBalance(node.Right) <= 0)
            return LeftRotate(node);
        if (balance < -1 && GetBalance(node.Right) > 0)
        {
            node.Right = RightRotate(node.Right!);
            return LeftRotate(node);
        }
        
        return node;
    }
    
    // LLM Optimization: Get tree height for performance monitoring
    public int GetTreeHeight()
    {
        return GetHeight(Root);
    }
    
    // LLM Optimization: Count total tasks for capacity planning
    public int CountNodes()
    {
        return CountNodesRecursive(Root);
    }
    
    private int CountNodesRecursive(OptimizedNode? node)
    {
        if (node == null) return 0;
        return 1 + CountNodesRecursive(node.Left) + CountNodesRecursive(node.Right);
    }
    
    // LLM Optimization: Check if tree is balanced (for monitoring)
    public bool IsBalanced()
    {
        return IsBalancedRecursive(Root);
    }
    
    private bool IsBalancedRecursive(OptimizedNode? node)
    {
        if (node == null) return true;
        
        int balance = GetBalance(node);
        return Math.Abs(balance) <= 1 && 
               IsBalancedRecursive(node.Left) && 
               IsBalancedRecursive(node.Right);
    }
    
    // LLM Optimization: Range query for tasks within priority range
    public void PrintTasksInRange(int min, int max)
    {
        Console.Write($"Tasks with priority {min}-{max}: ");
        PrintRangeRecursive(Root, min, max);
        Console.WriteLine();
    }
    
    private void PrintRangeRecursive(OptimizedNode? node, int min, int max)
    {
        if (node == null) return;
        
        if (min < node.Value)
            PrintRangeRecursive(node.Left, min, max);
        
        if (min <= node.Value && node.Value <= max)
            Console.Write(node.Value + " ");
        
        if (node.Value < max)
            PrintRangeRecursive(node.Right, min, max);
    }
}
