using MT.OnlineRestaurant.DataLayer.DataEntity;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.WindowsRuntime;

namespace MT.OnlineRestaurant.DataLayer
{
    public class RestaurantComparer : IEqualityComparer<RestaurantSearchDetails>
    {
        public bool Equals(RestaurantSearchDetails x, RestaurantSearchDetails y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x.restauran_ID == y.restauran_ID
                && x.restaurant_Name == y.restaurant_Name
                && x.restaurant_Address == y.restaurant_Address
                && x.restaurant_PhoneNumber == y.restaurant_PhoneNumber
                && x.restraurant_Website == y.restraurant_Website
                && x.closing_Time == y.closing_Time
                && x.opening_Time == y.opening_Time
                && x.xaxis == y.xaxis
                && x.yaxis == y.yaxis
                && x.rating == y.rating)
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(RestaurantSearchDetails obj)
        {
            return obj.restauran_ID.GetHashCode();
        }
    }
}
