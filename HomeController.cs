        public ActionResult Chat()
        {
            try
            {
                ViewData["senderName"] = GetUserID();
                //If user is logged in, disable the login prompt
                ViewBag.showLoginPrompt1 = "<script>\n$(window).load(function(){";
                ViewBag.showLoginPrompt2 = "$(\"input[type=submit]\").removeAttr('disabled')";
                ViewBag.showLoginPrompt3 = "$</script >";
                return View("Chat");
            }
            catch
            {
                // If user not logged in, display login prompt.
                ViewBag.showLoginPrompt1 = "<script>\n$(window).load(function(){";
                ViewBag.showLoginPrompt2 = "$(\"#myModal\").modal(\"show\");\n});";
                ViewBag.showLoginPrompt3 = "$(\"input[type = submit]\").attr('disabled','disabled');\n</script >";
                return View("Index");
            }
        }

        public string GetUserID()
        {
            // GET: Current Users ID
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

            // SET: UserID
            string userID = user.Id;

            return (userID);
        }
