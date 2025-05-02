using System;
using System.ComponentModel.DataAnnotations;

namespace LoginMVC.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        [Required(ErrorMessage = "OrderDate는 필수입니다.")]
        public DateTime OrderDate { get; set; } 

        [Required(ErrorMessage = "ShipName을 입력해주세요.")]
        public string ShipName { get; set; }

        [Required(ErrorMessage = "ShipAddress를 입력해주세요.")]
        public string ShipAddress { get; set; }

        [Required(ErrorMessage = "Freight는 필수 항목입니다.")]
        public decimal Freight { get; set; }


       
    }
}