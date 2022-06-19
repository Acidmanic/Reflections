using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Acidmanic.Utilities.Reflection.ObjectTree
{
    public class AddressKeyNodeMap : ThreeWayMap<AccessNode, FieldKey, string>
    {
        public AccessNode NodeByAddress(string address)
        {
            var index = IndexOf(address);

            if (index > -1)
            {
                return GetFirst(index);
            }

            return null;
        }

        public AccessNode NodeByKey(FieldKey key)
        {
            var index = IndexOf(key);

            if (index > -1)
            {
                return GetFirst(index);
            }

            return null;
        }

        public FieldKey FieldKeyByAddress(string address)
        {
            var index = IndexOf(address);

            if (index > -1)
            {
                return GetSecond(index);
            }

            return null;
        }

        public FieldKey FieldKeyByNode(AccessNode node)
        {
            var index = IndexOf(node);

            if (index > -1)
            {
                return GetSecond(index);
            }

            return null;
        }

        public string AddressByNode(AccessNode node)
        {
            var index = IndexOf(node);

            if (index > -1)
            {
                return GetThird(index);
            }

            return null;
        }

        public string AddressByKey(FieldKey key)
        {
            var index = IndexOf(key);

            if (index > -1)
            {
                return GetThird(index);
            }

            return null;
        }


        public List<AccessNode> Nodes => Firsts;
        public List<FieldKey> Keys => Seconds;
        public List<string> Addresses => Thirds;
    }
}