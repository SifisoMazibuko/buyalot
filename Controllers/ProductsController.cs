﻿using Buyalot.DbConnection;
using Buyalot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Buyalot.Controllers
{
    public class ProductsController : Controller
    {
        private DataContext Context { get; set; }
        private bool _DisposeContext = false;


        public ProductsController()
        {
            Context = new DataContext();
            _DisposeContext = true;
        }


        protected override void Dispose(bool disposing)
        {
            if (_DisposeContext)
                Context.Dispose();

            base.Dispose(disposing);

        }
        // GET: Product
        public ActionResult Index()
        {
            var db = new DataContext();
            ViewBag.ProductList = db.ProductModelSet.ToList();
            return View();
        }
        
        public ActionResult ViewProducts()
        {
            var product = (from p in Context.ProductModelSet
                           select p).ToList();
            return View(product);
        }
        
        //[HttpPost]
        //public ActionResult AddProduct(ProductModel model, HttpPostedFileBase productImage)
        //{

        //    var errors = ModelState
        //            .Where(x => x.Value.Errors.Count > 0)
        //            .Select(x => new { x.Key, x.Value.Errors })
        //             .ToArray();


        //    if (!ModelState.IsValid)
        //        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Error details");

        //    if (ModelState.IsValid)
        //        {
        //            var product = new ProductModel();
        //            product.productID = model.productID;
        //            product.productName = model.productName;
        //            product.productDescription = model.productDescription;
        //            product.vendor = model.vendor;
        //            product.price = model.price;
                
        //          if (productImage != null)
        //            {
        //            //   product.ImageMimeType = productImage.ContentType;
        //                product.productImage = new byte[productImage.ContentLength];
        //                productImage.InputStream.Read(product.productImage, 0, productImage.ContentLength);
        //            }


        //            Context.ProductModelSet.Add(product);
        //            Context.SaveChanges();

        //            return RedirectToAction("Index", "Home");
        //        }
        //       Context.SaveChanges();

        //    return View(model);
        //}

        //[HttpGet]
        //public ActionResult CategoryList(ProductCategoryModel prodCatId)
        //{
        //    var list =  (from c in Context.ProductCategoryModelSet
        //                select c).ToList();
        //    return View(list);
        //}

        public ActionResult AddProduct()
        {

            DataContext db = new DataContext();

            //IEnumerable<SelectListItem> items = new SelectList(db.ProductCategoryModelSet, "prodCategoryID", "categoryName");
            //ViewBag.prodCategoryID = items;

            ProductModel pro = new ProductModel();
            return View(pro);
        }


        [HttpPost]
        public ActionResult AddProduct(ProductModel product, HttpPostedFileBase upload)
        {
            var db = new DataContext();
                        
            if (upload != null)
            {
                product.productImage = new byte[upload.ContentLength];
                upload.InputStream.Read(product.productImage, 0, upload.ContentLength);
            }
            db.ProductModelSet.Add(product);
            db.SaveChanges();

            return View(product);
        }


        public ActionResult populateDropDown()
        {
            var db = new DataContext();
            
            List<SelectListItem> CustIDlistItems = new List<SelectListItem>();

            var getCategory = db.ProductCategoryModelSet.ToList();
            foreach (var category in getCategory)
            {
                CustIDlistItems.Add(new SelectListItem
                {
                    Text = category.categoryName,
                    Value = category.prodCategoryID.ToString()
                });
            }

            ViewBag.CatList = CustIDlistItems;
            return View();
        }

        private int itemExist(int id)
        {
            List<Item> cart = (List<Item>)Session["Cart"];
            int counter = cart.Count;
            for (int i = 0; i < counter; i++)
            {
                if (cart[i].Prdcts.productID == id)
                {
                    return i;
                }
            }
            return -1;

        }

        //[Authorize(Roles = "Customer")]
        public ActionResult AddToCart(int id)
        {
            var db = new DataContext();

            if (Session["Cart"] == null)
            {
                List<Item> cart = new List<Item>();
                cart.Add(new Item(db.ProductModelSet.Find(id), 1));

                Session["Cart"] = cart;
                Session["CartCounter"] = cart.Count;
            }
            else
            {
                List<Item> cart = (List<Item>)Session["Cart"];

                int index = itemExist(id);

                if (index == -1)
                {
                    cart.Add(new Item(db.ProductModelSet.Find(id), 1));
                    Session["CartCounter"] = cart.Count;
                }
                else
                {
                    cart[index].Quantity++;

                    if (Session["cartcounter"] == null)
                    {
                        Session["CartCounter"] = cart.Count;
                    }
                    else
                    {

                        var session = Session["cartcounter"];
                        Session["cartcounter"] = Convert.ToInt32(session) + cart[index].Quantity;
                    }

                }
                
                Session["Cart"] = cart;
            }

            return View("AddToCart");
        }

        public ActionResult DeleteCart(int id)
        {
            List<Item> cart = (List<Item>)Session["Cart"];
            int index = itemExist(id);


            cart.RemoveAt(index);

            Session["Cart"] = cart;

            return View("AddToCart");
        }

        public ActionResult UpdateQuantity(FormCollection fc)
        {
            string[] quantities = fc.GetValues("txtQuantity");
            List<Item> cart = (List<Item>)Session["Cart"];
            int counter = cart.Count;

            for (int i = 0; i < counter; i++)
            {
                cart[i].Quantity = Convert.ToInt32(quantities[i]);
                Session["Cart"] = cart;
            }



            return View("AddToCart");
        }

        public ActionResult UpdateCartCounter()
        {

            List<Item> cart = (List<Item>)Session["Cart"];
            int counter = cart.Count;

            for (int i = 0; i < counter; i++)
                cart[i].Quantity++;
            Session["Cart"] = cart;

            return View("AddToCart");
        }

    }
}