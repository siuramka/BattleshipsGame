﻿using backend.Models.Entity.Ships;

namespace backend.Models.Entity.Proxy
{
    class RealPlayerImage : IGameAsset
    {
        private Player player;
        Random random = new Random();


        public RealPlayerImage(Player player)
        {
            this.player = player;
            this.player.Icon = random.Next(1, 7).ToString();
        }

        public void GetImage()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(basePath, String.Format(@"icons\players\{0}.png", player.Icon));
            byte[] imageArray = File.ReadAllBytes(filePath);
            player.Icon = Convert.ToBase64String(imageArray);
        }
    }
}
