using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Haystack
{
    public static class Comparers
    {
        public static readonly IComparer<CelestialBody> DefaultCelestialComparer =
            new RUIutils.FuncComparer<CelestialBody>(
                (a, b) => StringComparer.CurrentCulture.Compare(a.bodyName, b.bodyName));

        public interface IPriorityComparer<T> : IComparer<T>
        {
            int Priority
            {
                get;
            }
        }

        public interface IDecoratorComparer<T> : IComparer<T>
        {
            IComparer<T> Decoratee { get; } 
        }

        public class PriorityDecoratorComparer<T> : IPriorityComparer<T>, IDecoratorComparer<T>
        {
            private IComparer<T> comparer;

            private PriorityDecoratorComparer(IComparer<T> comparer, int priority)
            {
                this.comparer = comparer;
                this.Priority = priority;
            }
            public int Compare(T x, T y)
            {
                return this.comparer.Compare(x, y);
            }

            public int Priority { get; private set; }

            public static IPriorityComparer<V> Decorate<V>(IComparer<V> comparer, int priorty)
            {
                return new PriorityDecoratorComparer<V>(comparer, priorty);
            }

            public IComparer<T> Decoratee { get { return this.comparer; } }
        }


        public class ComparerComparer<T> : IComparer<IComparer<T>>
        {
            public int Compare(IComparer<T> x, IComparer<T> y)
            {
                IPriorityComparer<T> pX = x as IPriorityComparer<T>;
                IPriorityComparer<T> pY = y as IPriorityComparer<T>;

                if (pX == null && pY == null)
                {
                    return 0;
                }

                if (pX != null && pY != null)
                {
                    // high priorty first
                    int diff = pY.Priority - pX.Priority;
                    return Math.Sign(diff);
                }

                //non IPriorityComparers have highest priority
                if (pX == null)
                {
                    return -1;
                }

                return 1;
            }
        }

        public class CombinedComparer<T> : IComparer<T>
        {
            private readonly List<IComparer<T>> comparers;

            public static CombinedComparer<V> FromList<V>(List<IComparer<V>> list)
            {
                return new CombinedComparer<V>(list);
            }
            
            public static CombinedComparer<V> Combine<V>(IComparer<V> one, IComparer<V> two)
            {
                CombinedComparer<V> first = one as CombinedComparer<V>;
                CombinedComparer<V> second = two as CombinedComparer<V>;

                if (first != null && second != null)
                {
                    return new CombinedComparer<V>(first.comparers.Union(second.comparers).ToList());
                }

                if (first != null)
                {
                    return first.Add(two);
                }

                if (second != null)
                {
                    return second.Add(one);
                }

                return new CombinedComparer<V>(new List<IComparer<V>> {one, two});
            }

            public static CombinedComparer<V> FromOne<V>(IComparer<V> comparer)
            {
                List<IComparer<V>> list = new List<IComparer<V>> { comparer };
                return new CombinedComparer<V>(list);
            }

            private CombinedComparer(List<IComparer<T>> comparers)
            {
                this.comparers = comparers;
                this.sortComparers();
            }

            private CombinedComparer(IEnumerable<IComparer<T>> comparers, IComparer<T> comparer)
            {
                this.comparers = new List<IComparer<T>>(comparers);
                this.comparers.Add(comparer);
                this.sortComparers();
            }

            private void sortComparers()
            {
                this.comparers.Sort(new ComparerComparer<T>());
            }

            public CombinedComparer<T> Add(IComparer<T> comparer)
            {
                return new CombinedComparer<T>(this.comparers, comparer);
            }

            public CombinedComparer<T> Remove<V>() where V : IComparer<T>
            {
                List<IComparer<T>> removed = this.comparers.ToList();
                removed.RemoveAll(c => c is V);

                return new CombinedComparer<T>(removed);
            }

            public ReadOnlyCollection<IComparer<T>> Comparers
            {
                get { return this.comparers.AsReadOnly(); }
            }

            public int Compare(T x, T y)
            {
                foreach (IComparer<T> comparer in comparers)
                {
                    int sign = Math.Sign(comparer.Compare(x, y));
                    if (sign != 0)
                    {
                        return sign;
                    }
                }
                return 0;
            }
        }

        public class FilteredStringComparer : IComparer<string>
        {
            private string filter;

            public FilteredStringComparer(string filter)
            {
                this.filter = filter;
            }
            public int Compare(string x, string y)
            {
                if (string.IsNullOrEmpty(x)) return -1;
                if (string.IsNullOrEmpty(y)) return 1;

                int xPos = x.IndexOf(this.filter, StringComparison.OrdinalIgnoreCase);
                int yPos = y.IndexOf(this.filter, StringComparison.OrdinalIgnoreCase);

                if (xPos >= 0 && yPos >= 0)
                {
                    int diff = xPos - yPos;
                    return Math.Sign(diff);
                }

                if (xPos >= 0)
                {
                    return -1;
                }

                if (yPos >= 0)
                {
                    return 1;
                }

                return 0;
            }
        }

        public class FilteredVesselComparer : IPriorityComparer<Vessel>
        {
            private readonly FilteredStringComparer filteredStringComparer;

            internal FilteredVesselComparer(string filter)
            {
                this.filteredStringComparer = new FilteredStringComparer(filter);
            }
            public int Compare(Vessel x, Vessel y)
            {
                return this.filteredStringComparer.Compare(x.vesselName, y.vesselName);
            }

            public int Priority
            {
                get { return 3; }
            }
        }

        public class VesselNameComparer : IPriorityComparer<Vessel>
        {
            public int Compare(Vessel x, Vessel y)
            {
                return StringComparer.OrdinalIgnoreCase.Compare(x.vesselName, y.vesselName);
            }

            public int Priority
            {
                get { return 1; }
            }
        }

        public class VesselLoadedComparer : IPriorityComparer<Vessel>
        {
            public int Compare(Vessel x, Vessel y)
            {
                if (x == null)
                {
                    return -1;
                }

                if (y == null)
                {
                    return 1;
                }

                if (x.loaded && y.loaded)
                {
                    return 0;
                }

                if (x.loaded)
                {
                    return 1;
                }

                if (y.loaded)
                {
                    return -1;
                }

                return 0;
            }

            public int Priority { get { return 3;  } }
        }

        public class VesselNearbyComparer : IPriorityComparer<Vessel>
        {
            private readonly Vessel compareVessel;

            public VesselNearbyComparer(Vessel vessel)
            {
                this.compareVessel = vessel;
            }
            public int Compare(Vessel x, Vessel y)
            {
                if (!HSUtils.IsInFlight || compareVessel == null)
                {
                    return 0;
                }

                if (x.loaded && y.loaded)
                {
                    return 0;
                }
                if (x.loaded)
                {
                    return -1;
                }
                if (y.loaded)
                {
                    return 1;
                }


                float distanceX = Vector3.Distance(compareVessel.transform.position, x.transform.position);
                float distanceY = Vector3.Distance(compareVessel.transform.position, y.transform.position);

                float diff = distanceX - distanceY;

                return Math.Abs(diff) <= 0.001f ? 0 : Math.Sign(diff);
            }

            public int Priority { get { return 3; } }
        }

        public class VesselMissionTimeComparer : IComparer<Vessel>
        {
            public int Compare(Vessel x, Vessel y)
            {
                if (x == null)
                {
                    return -1;
                }

                if (y == null)
                {
                    return 1;
                }

                double error = x.missionTime - y.missionTime;

                return Math.Abs(error) <= 0.00001d ? 0 : Math.Sign(error);
            }
        }


        public class ReverseComparer<T> : IDecoratorComparer<T>
        {
            internal ReverseComparer(IComparer<T> comparer)
            {
                this.Decoratee = comparer;
            }
            public int Compare(T x, T y)
            {
                return -this.Decoratee.Compare(x, y);
            }

            public IComparer<T> Decoratee { get; private set; }
        }

        public static IComparer<String> SortSubstringOrderComparer(string substring)
        {
            return new FilteredStringComparer(substring);
        }

        public static IComparer<T> Reverse<T>(this IComparer<T> orig)
        {
            return new ReverseComparer<T>(orig);
        }
    }
}
