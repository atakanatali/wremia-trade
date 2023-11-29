namespace WremiaTrade.Utilities.Attributes
{
    using System;

    public class OrderAttribute : Attribute
    {
        public OrderAttribute(int value = 0)
        {
            Order = value;
        }
        public int Order { get; private set; }
    }
}