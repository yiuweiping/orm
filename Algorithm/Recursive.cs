using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhengdi.Framework.Algorithm
{

    public interface ITreeNode
    {
        int Id { get; set; }
        string Name { get; set; }
        int Parent { get; set; }

    }
    public interface IBinaryTree: ITreeNode
    {
        IList<IBinaryTree> Child { get; set; }
    }
    public class Recursive<T,K> where T : class, IBinaryTree, new() where K : class, ITreeNode, new()
    {
        public IList<IBinaryTree> Run(IEnumerable<K> source, Func<K, T> compatison, IBinaryTree parent = null) 
        {
            var list = new List<IBinaryTree>();
            parent = parent ?? new T() { Id = 0, Parent = 0 };
            Calculate(source, parent,list, compatison);
            return list;
        }
        static void Calculate(IEnumerable<K> source, IBinaryTree parent, IList<IBinaryTree> target, Func<K, T> compatison)
        {
            IBinaryTree tree = null;
            foreach (var e in source)
            {
                if (null != parent)
                {
                    if (parent.Id == e.Id)
                    {
                        tree = compatison(e as K);
                        parent.Child.Add(tree);
                        Calculate(source, tree, target, compatison);
                    }
                    else
                    {
                        tree = compatison(e as K);
                        target.Add(tree);
                        Calculate(source, tree, target, compatison);
                    }
                }

            }
        }

    }
}
