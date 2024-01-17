using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Models;
using Microsoft.Ajax.Utilities;

namespace Inventory.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private InventoryContext db = new InventoryContext();

        public ActionResult Index()
        {
            // Obtener el nombre del usuario y el ID del inventario
            string userName = db.Users.Find(int.Parse(User.Identity.Name)).Name;
            int userId = int.Parse(User.Identity.Name);
            int inventoryId = GetInventoryId(userId);
            List<Location> locations = GetLocationsForInventory(inventoryId);
            List<List<Category>> categories = GetCategoriesByLocation(inventoryId);
            List<List<List<Item>>> items = GetItemsbyCategory(inventoryId);
            List<PerishableItem> perishableItems = GetPerishableItems(inventoryId);
            // Crear el mensaje de saludo
            string greetingMessage = $"Hello, {userName}! Your Inventory ID is {inventoryId}.";

            // Pasar el mensaje de saludo a la vista
            ViewBag.GreetingMessage = greetingMessage;
            ViewBag.Locations = locations;
            ViewBag.Categories = categories;
            ViewBag.Items = items;
            ViewBag.Dates = perishableItems;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        // Método para obtener el ID del inventario del usuario
        private int GetInventoryId(int userId)
        {
            var userInventory = db.Inventories.FirstOrDefault(inv => inv.UserID == userId);
            return userInventory?.InventoryID ?? 0;
        }
        private List<Location> GetLocationsForInventory(int inventoryId)
        {
            return db.Locations.Where(loc => loc.InventoryID == inventoryId).ToList();
        }
        [HttpPost]
        public JsonResult AddLocation(string name, string description)
        {
            int userId = int.Parse(User.Identity.Name);
            int inventoryId = GetInventoryId(userId);

            Location newLocation = new Location
            {
                Name = name,
                Description = description,
                InventoryID = inventoryId
            };

            db.Locations.Add(newLocation);
            db.SaveChanges();

            return Json(new { success = true, message = "Location added successfully" });
        }
        [HttpPost]
        public JsonResult EditLocation(int id, string name, string description)
        {
            try
            {
                // Obtener la ubicación a editar
                var location = db.Locations.FirstOrDefault(loc => loc.LocationID == id);

                if (location != null)
                {
                    // Actualizar los datos de la ubicación
                    location.Name = name;
                    location.Description = description;

                    // Guardar los cambios en la base de datos
                    db.SaveChanges();

                    // Devolver un JsonResult exitoso
                    return Json(new { success = true, message = "Location edited successfully" });
                }
                else
                {
                    // Devolver un JsonResult con un mensaje de error si la ubicación no se encuentra
                    return Json(new { success = false, message = "Location not found" });
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier error y devolver un JsonResult con un mensaje de error
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        private List<List<Category>> GetCategoriesByLocation(int inventoryId)
        {
            var Locations = GetLocationsForInventory(inventoryId);
            List<List<Category>> categories = new List<List<Category>>();
            foreach (var location in Locations)
            {
                List<Category> category = new List<Category>(); //categoria dinamica para cada localizacion
                category = db.Categories.Where(cat => cat.LocationID == location.LocationID).ToList();
                categories.Add(category);
            }
            return categories;
        }
        [HttpPost]
        public JsonResult AddCategory(string name, string description, int locationId)
        {
            try
            {
                // Verificar si la ubicación pertenece al usuario actual
                int userId = int.Parse(User.Identity.Name);
                int inventoryId = GetInventoryId(userId);

                var location = db.Locations.FirstOrDefault(loc => loc.LocationID == locationId && loc.InventoryID == inventoryId);

                if (location == null)
                {
                    return Json(new { success = false, message = "Invalid location ID" });
                }

                // Crear una nueva categoría
                Category newCategory = new Category
                {
                    Name = name,
                    Description = description,
                    LocationID = locationId
                };

                // Agregar la categoría a la base de datos
                db.Categories.Add(newCategory);
                db.SaveChanges();

                // Devolver un JsonResult exitoso
                return Json(new { success = true, message = "Category added successfully" });
            }
            catch (Exception ex)
            {
                // Manejar cualquier error y devolver un JsonResult con un mensaje de error
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        [HttpPost]
        public JsonResult EditCategory(int categoryid, string name, string description)
        {
            try
            {
                // Obtener la categoría a editar
                var category = db.Categories.FirstOrDefault(cat => cat.CategoryID == categoryid);
                if (category != null)
                {
                    // Actualizar los datos de la categoría
                    category.Name = name;
                    category.Description = description;
                    // Guardar los cambios en la base de datos
                    db.SaveChanges();
                    // Devolver un JsonResult exitoso
                    return Json(new { success = true, message = "Category edited successfully" });
                }
                else
                {
                    // Devolver un JsonResult con un mensaje de error si la categoría no se encuentra
                    return Json(new { success = false, message = "Category not found" });
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                // Manejar cualquier error y devolver un JsonResult con un mensaje de error
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        [HttpPost]
        public JsonResult AddItem(int categoryId, string itemName, string itemDescription,int quantity, bool perishable, DateTime? expirationDate)
        {
            try
            {
                // Verificar si la categoría pertenece al usuario actual
                int userId = int.Parse(User.Identity.Name);
                int inventoryId = GetInventoryId(userId);

                var category = db.Categories.FirstOrDefault(cat => cat.CategoryID == categoryId && cat.Location.InventoryID == inventoryId);

                if (category == null)
                {
                    return Json(new { success = false, message = "Invalid category ID" });
                }

                // Crear un nuevo ítem
                Item newItem = new Item
                {
                    CategoryID = categoryId,
                    Name = itemName,
                    Quantity = quantity,
                    IsPerishable = perishable,
                    Description = itemDescription,
                    //set default image as image path for now, ~/images/items/default.png
                    ImagePath = "~/images/items/default.png",
                };

                // Agregar el ítem a la base de datos
                db.Items.Add(newItem);
                db.SaveChanges();

                //buscar el id del igem recien creado
                var item = db.Items.FirstOrDefault(it => it.Name == itemName && it.CategoryID == categoryId);
                //si el item es perecedero, agregar objeto de fecha de expiracion a la db con user id y la fecha de expiracion
                if (perishable)
                {
                    PerishableItem perishableItem = new PerishableItem();
                    perishableItem.ItemID = item.ItemID;
                    perishableItem.ExpirationDate = expirationDate; 
                    db.PerishableItems.Add(perishableItem);
                    db.SaveChanges();
                }


                // Devolver un JsonResult exitoso
                return Json(new { success = true, message = "Item added successfully" });
            }
            catch (Exception ex)
            {
                // Manejar cualquier error y devolver un JsonResult con un mensaje de error
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public JsonResult EditItem(int itemId, string itemName, string itemDescription, int quantity, bool perishable, DateTime? expirationDate)
        {
            try
            {
                //begin by finding the item to edit
                var item = db.Items.FirstOrDefault(it => it.ItemID == itemId);
                if (item != null)
                {
                    //update the item's data
                    item.Name = itemName;
                    item.Description = itemDescription;
                    item.Quantity = quantity;
                    item.IsPerishable = perishable;
                    //if the item is perishable, update the expiration date
                    if (perishable)
                    {
                        var perishableItem = db.PerishableItems.FirstOrDefault(pi => pi.ItemID == itemId);
                        //check if date changed, if not, don't update
                        if (perishableItem.ExpirationDate != expirationDate)
                        {
                            perishableItem.ExpirationDate = expirationDate;
                        }
                    }
                    //save changes to the database
                    db.SaveChanges();
                    //return a successful JsonResult
                    return Json(new { success = true, message = "Item edited successfully" });
                }
                else
                {
                    //return a JsonResult with an error message if the item is not found
                    return Json(new { success = false, message = "Item not found" });
                }   
            }
            catch (Exception ex)
            {
                string error = ex.Message;

                // En caso de error
                return Json(new { success = false, message = "Error editing item: " + ex.Message });
            }
        }


        private List<List<List<Item>>> GetItemsbyCategory(int inventoryId)
        {
            var Locations = GetLocationsForInventory(inventoryId);
            List<List<List<Item>>> items = new List<List<List<Item>>>();
            foreach (var location in Locations)
            {
                var Categories = db.Categories.Where(cat => cat.LocationID == location.LocationID).ToList();
                List<List<Item>> categoryItems = new List<List<Item>>();
                foreach (var category in Categories)
                {
                    List<Item> item = new List<Item>();
                    item = db.Items.Where(it => it.CategoryID == category.CategoryID).ToList();
                    categoryItems.Add(item);
                }
                items.Add(categoryItems);
            }
            return items;
        }
        private List<PerishableItem> GetPerishableItems(int inventoryid)
        {
            var Locations = GetLocationsForInventory(inventoryid);
            List<PerishableItem> perishableItems = new List<PerishableItem>();
            foreach (var location in Locations)
            {
                var Categories = db.Categories.Where(cat => cat.LocationID == location.LocationID).ToList();
                foreach (var category in Categories)
                {
                    var Items = db.Items.Where(it => it.CategoryID == category.CategoryID).ToList();
                    foreach (var item in Items)
                    {
                        var perishableItem = db.PerishableItems.FirstOrDefault(pi => pi.ItemID == item.ItemID);
                        if (perishableItem != null)
                        {
                            perishableItems.Add(perishableItem);
                        }
                    }
                }
            }
            return perishableItems;
        }
        [HttpPost]
        public ActionResult UploadImage()
        {
            try
            {
                var itemId = Request.Form["itemId"];
                var file = Request.Files[0];
                var numericid = int.Parse(itemId);
                var item = db.Items.FirstOrDefault(it => it.ItemID == numericid);
                if (item == null)
                {
                    return Json(new { success = false, message = "Invalid item ID" });
                }
                //renombrar al file con el id del item y extension .png
                var fileName = itemId + ".png";
                //guardar el file en la carpeta de imagenes (sobreescribir si ya existe)
                file.SaveAs(Server.MapPath("~/images/items/" + fileName));
                //actualizar el path de la imagen en la db
                item.ImagePath = "~/images/items/" + fileName;
                db.SaveChanges();

                return Json(new { success = true, message = "Imagen cargada exitosamente." });
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                string stackTrace = ex.StackTrace;
                // Maneja errores
                return Json(new { success = false, message = "Error al cargar la imagen." });
            }
        }
        //void to delete an item and its perishable item if it has one
        void DeleteItemDB(int itemid)
        {
            var item = db.Items.FirstOrDefault(it => it.ItemID == itemid);
            if (item != null)
            {
                if (item.IsPerishable==true)
                {
                    var perishableItem = db.PerishableItems.FirstOrDefault(pi => pi.ItemID == itemid);
                    if (perishableItem != null)
                    {
                        db.PerishableItems.Remove(perishableItem);
                        db.SaveChanges();
                    }
                }
                db.Items.Remove(item);
                db.SaveChanges();
            }
        }
        //void to delete a category and use category id to delete all items in that category
        void DeleteCategoryDB(int categoryid)
        {
            var category = db.Categories.FirstOrDefault(cat => cat.CategoryID == categoryid);
            if (category != null)
            {
                var items = db.Items.Where(it => it.CategoryID == categoryid).ToList();
                foreach (var item in items)
                {
                    DeleteItemDB(item.ItemID);
                }
                db.Categories.Remove(category);
                db.SaveChanges();
            }
        }
        //void to delete a location and use location id to delete all categories in that location
        void DeleteLocationDB(int locationid)
        {
            var location = db.Locations.FirstOrDefault(loc => loc.LocationID == locationid);
            if (location != null)
            {
                var categories = db.Categories.Where(cat => cat.LocationID == locationid).ToList();
                foreach (var category in categories)
                {
                    DeleteCategoryDB(category.CategoryID);
                }
                db.Locations.Remove(location);
                db.SaveChanges();
            }
        }
        //jsonresult to delete an item
        [HttpPost]
        public JsonResult DeleteItem(int itemid)
        {
            try
            {
                DeleteItemDB(itemid);
                return Json(new { success = true, message = "Item deleted successfully" });
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return Json(new { success = false, message = "Error deleting item: " + ex.Message });
            }
        }
        //jsonresult to delete a category
        [HttpPost]
        public JsonResult DeleteCategory(int categoryid)
        {
            try
            {
                DeleteCategoryDB(categoryid);
                return Json(new { success = true, message = "Category deleted successfully" });
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return Json(new { success = false, message = "Error deleting category: " + ex.Message });
            }
        }
        //jsonresult to delete a location
        [HttpPost]
        public JsonResult DeleteLocation(int locationid)
        {
            try
            {
                DeleteLocationDB(locationid);
                return Json(new { success = true, message = "Location deleted successfully" });
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return Json(new { success = false, message = "Error deleting location: " + ex.Message });
            }
        }
    }
}