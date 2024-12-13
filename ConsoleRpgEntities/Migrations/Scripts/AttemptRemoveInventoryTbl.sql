ALTER TABLE [dbo].[Inventory] DROP CONSTRAINT [FK_Inventory_Player_PlayerId];
ALTER TABLE [dbo].[Items] DROP CONSTRAINT [FK_Items_Inventory_InventoryId];
DROP TABLE Inventory;