// AuthController.cs en la carpeta Controllers
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Inventory.Models;


public class AuthController : Controller
{
    // En lugar de la lista estática, utiliza el contexto de la base de datos
    private InventoryContext db = new InventoryContext();
    [AllowAnonymous]
    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public ActionResult Login(string email, string password)
    {
        var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

        if (user != null)
        {
            FormsAuthentication.SetAuthCookie(user.UserID.ToString(), false);
            return RedirectToAction("Index", "Home");
        }

        ViewBag.ErrorMessage = "Credenciales inválidas.";
        return View();
    }

    [AllowAnonymous]
    public ActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public ActionResult Register(User user)
    {
        if (ModelState.IsValid)
        {
            // Registra al usuario en la base de datos
            db.Users.Add(user);
            db.SaveChanges();

            // Ahora, puedes acceder al UserID asignado después de llamar a SaveChanges()
            int userId = user.UserID;

            // Crea un nuevo inventario asociado al usuario recién registrado
            Inventory.Models.Inventory newInventory = new Inventory.Models.Inventory();

            newInventory.UserID = userId;
            db.Inventories.Add(newInventory);
            db.SaveChanges();

            FormsAuthentication.SetAuthCookie(userId.ToString(), false);
            return RedirectToAction("Index", "Home");
        }

        ViewBag.ErrorMessage = "Invalid credentials.";
        return View(user);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult LogOff()
    {
        FormsAuthentication.SignOut();
        return RedirectToAction("Login", "Auth");
    }
}
