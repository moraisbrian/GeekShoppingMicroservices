﻿namespace GeekShopping.CartAPI.DTOs
{
    public class CouponDTO
    {
        public long Id { get; set; }
        public string CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}