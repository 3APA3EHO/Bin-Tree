using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BinaryTree
{
    [Serializable]
    [DebuggerDisplay("Value = {Value}")]
    class Branch<T>
    {
        public Branch(T Value) => this.Value = Value;
        public T Value { get; private set; }
        internal Branch<T> l { get; set; }
        internal Branch<T> r { get; set; }
        public Branch<T> Find(T value)
        {
            try
            {
                if (value.GetHashCode() > ((T)this).GetHashCode())
                    return l.Find(value);
                else if (value.GetHashCode() < ((T)this).GetHashCode())
                    return r.Find(value);
                return this;
            }
            catch
            {
                //TODO: FIX THIS
                throw new Exception("Element is not in the tree");
            }
        }
        public void Add(T newElement)
        {
            if (newElement.GetHashCode() > ((T)this).GetHashCode())
            {
                if (l is null)
                    l = new Branch<T>(newElement);
                else
                    l.Add(newElement);
            }
            else if (newElement.GetHashCode() < ((T)this).GetHashCode())
            {
                if (r is null)
                    r = new Branch<T>(newElement);
                else
                    r.Add(newElement);
            }
            else
                throw new Exception("Element is in the tree");
        }

        public static explicit operator T(Branch<T> b) => b.Value;

        public List<HashSet<T>> Levels
        {
            get
            {
                List<HashSet<Branch<T>>> output = new List<HashSet<Branch<T>>>();

                output.Add(new HashSet<Branch<T>> {this}); // lvl 0

                //TODO: this need optimisation!
                int oldcount = 0;
                int newcount = 1;
                while (oldcount != newcount)
                {
                    oldcount = newcount;

                    for(int i = 0; i < output.Count(); i++)
                    {
                        foreach(var t in output[i])
                        {
                            if (t.l != null)
                            {
                                if (output.Count == i + 1)
                                    output.Add(new HashSet<Branch<T>>());
                                output[i + 1].Add(t.l);
                            }
                            if (t.r != null)
                            {
                                if (output.Count == i + 1)
                                    output.Add(new HashSet<Branch<T>>());
                                output[i + 1].Add(t.r);
                            }
                        }
                    }

                    newcount = output.SelectMany(x => x).Count();
                }

                return output.Select(x=> x.Select(y=> (T)y).ToHashSet()).ToList();
            } }
        public List<T> ToList(){ return Levels.SelectMany(x => x).ToList(); }
        public Branch<T> ReBalance() { 
                var allelements = ToList();
                if (allelements.Count() < 4 ) 
                    return this; // no need of rebalance
            return ReBalance(allelements);
            }

        public Branch<T> ReBalance(List<T> allelements)
        {
            //TODO: add log check for efficiency
            if (allelements.Count == 0)
                throw new ArgumentException("List is empty!");

            allelements.OrderBy(x => x.GetHashCode()).ToList();
            Branch<T> root = new Branch<T>(allelements[(int)Math.Floor(allelements.Count() / 2d)]);
            allelements.Remove((T)root);
            allelements.OrderBy(x => Guid.NewGuid());
            foreach (T t in allelements)
                root.Add(t);
            return root;
        }
        public Branch<T> Remove(T value)
        {
            Branch<T> element;
            try
            {
                element = Find(value);
            }
            catch
            {
                return this;//element is not in the tree
            }
            List<T> elements = element.ToList().Select(x => (T)x).ToList();
            elements.Remove((T)element);
            if (elements.Count() == 0)
                throw new Exception("Can not delete all elements");
            return ReBalance(elements);

        }
    }
}
