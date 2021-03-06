﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Mag.BusinessLayer;
using System.IO;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Text.RegularExpressions;

namespace Mag
{
    public partial class Default : System.Web.UI.Page
    {
        int pos;
        int last;
        PagedDataSource adsource;
        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_Count.Text = Application["Visitors"].ToString() + "/" + Application["myCounter"].ToString();
            lblLogo.Text = lblFooter.Text = ConfigurationManager.AppSettings["NameOfShop"];
            lblDesc.Text = ConfigurationManager.AppSettings["Description"];
            lblTelefon1.Text = ConfigurationManager.AppSettings["Telefon"];
            pageall.Visible = false;
            if (!IsPostBack)
            {
                this.ViewState["vs"] = 0;
                this.ViewState["vf"] = 0;
                lblCategoryName.Text = "";
                GetCategory();
                GetProducts(0);
            }
            pos = (int)this.ViewState["vs"];
            last = (int)this.ViewState["vf"];
            btnFirstPage.Text = Convert.ToString(pos + 1);
            btnLastPage.Text = Convert.ToString(last + 1);

        }
        protected void Page_Loaded(object sender, EventArgs e)
        {
            //ScriptManager scriptmanager1 = Page.FindControl("ScriptManager1") as ScriptManager;
            ScriptManager1.AddHistoryPoint(pageKey, "0");
        }

        protected void lblLogo_Click(object sender, EventArgs e)
        {
            lblCategoryName.Text = "";
            lbtProducts.Text = "Наш каталог";
            pnlCategories.Visible = true;
            pnlProducts.Visible = true;
            pnlDetails.Visible = false;
            pnlCheckOut.Visible = false;

            GetProducts(0);
        }

        protected void btnShoppingCart_Click(object sender, EventArgs e)
        {
            lblCategoryName.Text = "";
            lbtProducts.Text = "";
            pnlCheckOut.Visible = true;
            pnlDetails.Visible = false;
            pnlCategories.Visible = false;
            pnlProducts.Visible = false;
        }


        private DataTable GetData(string query)
        {
            DataTable dt = new DataTable();
            string Conn = WebConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
            SqlConnection con = new SqlConnection(Conn);
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            da.Fill(dt);
            con.Close();
            return dt;
        }

        protected void lbtnCategory_Click(object sender, EventArgs e)
        {
            pnlProducts.Visible = true;
            pnlDetails.Visible = false;

            int CategoryID = Convert.ToInt16((((LinkButton)sender).CommandArgument));
            this.ViewState["vs"] = 0;
            pos = 0;
            GetProducts(CategoryID);
            // HighlightCartProducts();
            //ScriptManager scriptmanager1 = Page.FindControl("ScriptManager1") as ScriptManager;
            ScriptManager1.AddHistoryPoint(modeKey, Convert.ToString(CategoryID));
        }

        private void GetCategory()
        {
            ShoppingCart k = new ShoppingCart();
            dlCategories.DataSource = null;
            dlCategories.DataSource = k.GetCategories();
            dlCategories.DataBind();
        }

        private void GetProducts(int CategoryIDs)
        {
            pageall.Visible = true;
            ShoppingCart k = new ShoppingCart()
            {
                CategoryID = CategoryIDs
            };

            dlProducts.DataSource = null;
            adsource = new PagedDataSource();
            adsource.DataSource = (k.GetAllProducts()).DefaultView;
            if (CategoryIDs != 0 && pnlProducts.Visible == true)
            {
                DataTable dtable = k.GetCategory();
                if (dtable.Rows.Count > 0)
                {
                    lblCategoryName.Text = "  >  " + (k.GetCategory()).Rows[0]["CategoryName"].ToString() + " >";
                }
            }
            else lblCategoryName.Text = "";
            adsource.AllowPaging = true;
            adsource.PageSize = 9;
            adsource.CurrentPageIndex = pos;
            //btnfirst.Enabled = !adsource.IsFirstPage;

            //btnlast.Enabled = !adsource.IsLastPage;
            last = adsource.PageCount - 1;
            this.ViewState["vf"] = last;
            btnprevious.Visible = btnnext.Visible = btnLastPage.Visible = btnFirstPage.Visible = true;
            btnFirstPage.Text = "1";
            btnLastPage.Text = Convert.ToString(last + 1);
            btnprevious.Enabled = !adsource.IsFirstPage;
            btnnext.Enabled = !adsource.IsLastPage;

            if (adsource.IsLastPage)
            {
                btnLastPage.ForeColor = System.Drawing.Color.White;
                btnLastPage.Font.Bold = true;
            }
            else
            {
                btnLastPage.ForeColor = System.Drawing.Color.Black;
                btnLastPage.Font.Bold = false;
            }
            if (adsource.IsFirstPage)
            {
                btnFirstPage.ForeColor = System.Drawing.Color.White;
                btnFirstPage.Font.Bold = true;
            }
            else
            {
                btnFirstPage.ForeColor = System.Drawing.Color.Black;
                btnFirstPage.Font.Bold = false;
            }

            if (adsource.PageCount == 1)
            {
                pageall.Visible = false;
                btnFirstPage.Visible = false;
                btnLastPage.Visible = false;
                btnprevious.Visible = false;
                btnnext.Visible = false;
            }
            else
            {
                btnFirstPage.Visible = true;
                btnFirstPage.Enabled = !adsource.IsFirstPage;
                btnLastPage.Visible = true;
                btnLastPage.Enabled = !adsource.IsLastPage;
            }
            dlProducts.DataSource = adsource;
            dlProducts.DataBind();
        }

        //protected void btnfirst_Click(object sender, EventArgs e)
        //{
        //    pos = 0;
        //    GetProducts(0);
        //}

        protected void btnprevious_Click(object sender, EventArgs e)
        {
            pos = (int)this.ViewState["vs"];
            pos -= 1;
            this.ViewState["vs"] = pos;
            this.ViewState["vf"] = last;
            ScriptManager1.AddHistoryPoint(pageKey, Convert.ToString(pos));
            GetProducts(0);
        }

        protected void btnnext_Click(object sender, EventArgs e)
        {
            pos = (int)this.ViewState["vs"];
            pos += 1;
            this.ViewState["vs"] = pos;
            this.ViewState["vf"] = last;
            ScriptManager1.AddHistoryPoint(pageKey, Convert.ToString(pos));
            GetProducts(0);
        }

        //protected void btnlast_Click(object sender, EventArgs e)
        //{
        //    pos = adsource.PageCount - 1;
        //    GetProducts(0);
        //}

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            ScriptManager1.AddHistoryPoint(pageKey, "0");
            int ProductID = (int)Convert.ToInt32(((((Button)sender).CommandArgument)).ToString());
            NewMessage(ProductID);
        }

        protected void btnWriteMessage_Click(object sender, EventArgs e)
        {
            int ProductID = (int)Convert.ToInt32(((Button)sender).CommandArgument);
            NewMessage(ProductID);
        }

        private void NewMessage(int ProductID)
        {
            hfProductID.Value = ProductID.ToString();
            pnlCheckOut.Visible = true;
            pnlDetails.Visible = false;
            //pnlCategories.Visible = false;
            //pnlProducts.Visible = false;
        }


        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            pnlProducts.Visible = false;
            lblCategoryName.Text = "";
            pnlDetails.Visible = true;
            int ProductID = (int)Convert.ToInt32(((((ImageButton)sender).CommandArgument)).ToString());
            ScriptManager1.AddHistoryPoint(deltaKey, Convert.ToString(ProductID));
            ShoppingCart k = new ShoppingCart()
            {
                ProductID = ProductID
            };

            DataTable dtResult = k.GetProductInfo();
            lblProductName.Text = Convert.ToString(dtResult.Rows[0]["Name"]);
            lblProductDescription.Text = Convert.ToString(dtResult.Rows[0]["Description"]);
            lblPrice.Text = Convert.ToString(dtResult.Rows[0]["Price"]);
            lblTelefon2.Text = ConfigurationManager.AppSettings["Telefon"];
            //HiddenField1.Value = ProductID.ToString();
            btnWriteMessage.CommandArgument = ProductID.ToString();
            Image2.ImageUrl = Convert.ToString(dtResult.Rows[0]["ImageUrl"]);
            //        ProductID = Convert.ToInt32(dt.Rows[i]["ProductID"]),
            //        TotalProducts = Convert.ToInt32(dt.Rows[i]["ProductQuantity"])
            //    };

        }

        protected void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            //string productids = string.Empty;
            int ProductID = Convert.ToInt32(hfProductID.Value);
            //DataTable dt;
            if (txtCustomerName.Text != null && txtCustomerEmailID.Text != null)
            {
                //dt = (DataTable)Session["MyCart"];

                ShoppingCart k = new ShoppingCart()
                {
                    CustomerName = txtCustomerName.Text,
                    CustomerEmailID = txtCustomerEmailID.Text,
                    CustomerPhoneNo = txtCustomerPhoneNo.Text,
                    CustomerMessage = txtCustomerMessage.Text,
                    ProductID = ProductID
                    //TotalProducts = Convert.ToInt32(txtTotalProducts.Text),
                    //TotalPrice = Convert.ToInt32(txtTotalPrice.Text),
                    //ProductList = productids
                    //PaymentMethod = rblPaymentMethod.SelectedItem.Text
                };

                DataTable dtResult = k.SaveCustomerDetails();

                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    ShoppingCart SaveProducts = new ShoppingCart()
                //    {
                //        CustomerID = Convert.ToInt32(dtResult.Rows[0][0]),
                //        ProductID = Convert.ToInt32(dt.Rows[i]["ProductID"]),
                //        TotalProducts = Convert.ToInt32(dt.Rows[i]["ProductQuantity"])
                //    };
                //    SaveProducts.SaveCustomerProducts();
                //}

                Session.Clear();

                pnlCheckOut.Visible = false;
                pnlCategories.Visible = false;
                pnlProducts.Visible = false;

                SendOrderPlacedAlert(txtCustomerName.Text, txtCustomerEmailID.Text, ProductID);

                txtCustomerName.Text = string.Empty;
                txtCustomerEmailID.Text = string.Empty;
                txtCustomerPhoneNo.Text = string.Empty;
                txtCustomerMessage.Text = string.Empty;

                pnlProducts.Visible = true;
                pnlCategories.Visible = true;
                pnlCheckOut.Visible = false;
                pnlDetails.Visible = false;

                // Response.Redirect("~/Default.aspx");
            }
        }

        private void SendOrderPlacedAlert(string CustomerName, string CustomerEmailID, int ProductId)
        {
            string body = this.PopulateOrderEmailBody(CustomerName, ProductId);

            EmailEngine.SendEmail(ConfigurationManager.AppSettings["UserName"], "Письмо из магазина от " + txtCustomerName.Text, body);
        }

        private string PopulateOrderEmailBody(string customerName, int ProductId)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/OrderTemplate.htm")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{CustomerName}", txtCustomerName.Text);
            body = body.Replace("{ProductNo}", ProductId.ToString());
            body = body.Replace("{emailMessage}", txtCustomerMessage.Text);
            body = body.Replace("{mailEmail}", txtCustomerEmailID.Text);
            body = body.Replace("{PhoneNo}", txtCustomerPhoneNo.Text);
            return body;
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            txtCustomerName.Text = string.Empty;
            txtCustomerEmailID.Text = string.Empty;
            txtCustomerPhoneNo.Text = string.Empty;
            txtCustomerMessage.Text = string.Empty;

            pnlProducts.Visible = true;
            pnlCategories.Visible = true;
            pnlDetails.Visible = false;
            pnlCheckOut.Visible = false;

        }

        protected void lblProductDescription_PreRender(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            lbl.Text = (lbl.Text).Replace("\r\n", "<br/>");
        }

        protected void btnFirstPage_Click(object sender, EventArgs e)
        {
            pos = 0;
            this.ViewState["vs"] = pos;
            //ScriptManager scriptmanager1 = Page.FindControl("ScriptManager1") as ScriptManager;
            ScriptManager1.AddHistoryPoint(pageKey, Convert.ToString(pos));
            GetProducts(0);
        }

        protected void btnLastPage_Click(object sender, EventArgs e)
        {
            pos = (int)this.ViewState["vf"];
            this.ViewState["vs"] = pos;
            //ScriptManager scriptmanager1 = Page.FindControl("ScriptManager1") as ScriptManager;
            ScriptManager1.AddHistoryPoint(pageKey, Convert.ToString(pos));
            GetProducts(0);
        }

        private static string pageKey = "p";
        private static string modeKey = "m";
        private static string deltaKey = "d";

        protected void ScriptManager1_Navigate(object sender, HistoryEventArgs e)
        {
            if (e.State.Count <= 0)
            {
                this.ViewState["vs"] = 0;
                pnlCategories.Visible = true;
                pnlProducts.Visible = true;
                pnlDetails.Visible = false;
                pnlCheckOut.Visible = false;
                GetCategory();
                GetProducts(0);
                return;
            }

            string key = e.State.AllKeys[0];
            string state = String.Empty;

            if (String.Equals(key, pageKey))
            {
                state = e.State[key];
                int pageIndex = Convert.ToInt32(state);
                this.ViewState["vs"] = pageIndex;
                GetProducts(0);
            }
            if (String.Equals(key, modeKey))
            {
                state = e.State[key];
                GetProducts(Convert.ToInt32(state));
            }
            if (String.Equals(key, deltaKey))
            {
                state = e.State[key];
                GetProducts(Convert.ToInt32(state));
            }
        }

        protected void dlProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            string state = adsource.CurrentPageIndex.ToString();
            ScriptManager1.AddHistoryPoint(pageKey, state);
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            pnlProducts.Visible = false;
            lblCategoryName.Text = "";
            pnlDetails.Visible = true;
            NewMessage(0);
        }


    }
}