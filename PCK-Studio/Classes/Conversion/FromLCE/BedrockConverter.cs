using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OMI.Formats.Languages;
using OMI.Formats.Pck;
using OMI.Workers.Language;
using OMI.Workers.Pck;

using PckStudio.Classes.Utils;

namespace PckStudio.Classes.FromLCE
{
	internal class BedrockConverter
    {
        // this is just a placeholder in case the locfile entry fails
        // the placeholder name can be changed at any time
        string PackName = "pck_studio";
        // Geometry.json
        JObject GJSON = new JObject();

        static Dictionary<string, string> languageMap = new Dictionary<string, string>()
        {
            {"cs-CS", "cs_CZ"},
            {"cs-CZ", "cs_CZ"},

            {"da-CH", "da_DK"},
            {"da-DA", "da_DK"},
            {"da-DK", "da_DK"},

            {"de-AT", "de_DE"},
            {"de-DE", "de_DE"},

            {"el-EL", "el_GR"},
            {"el-GR", "el_GR"},

            {"en-AU", "en_GB"},
            {"en-CA", "en_US"},
            {"en-EN", "en_US"},
            {"en-GB", "en_GB"},
            {"en-GR", "en_GB"},
            {"en-IE", "en_GB"},
            {"en-NZ", "en_GB"},
            {"en-US", "en_US"},

            {"es-ES", "es_ES"},
            {"es-MX", "es_MX"},

            {"fi-BE", "fi_FI"},
            {"fi-CH", "fi_FI"},
            {"fi-FI", "fi_FI"},

            {"fr-FR", "fr_CA"},
            {"fr-CA", "fr_FR"},

            {"it-IT", "it_IT"},

            {"ja-JP", "ja_JP"},

            {"ko-KR", "ko_KR"},

            {"la-LAS", "la-LAS"},

            {"no-NO", "no-NO"},

            {"nb-NO", "nb_NO"},

            {"nl-NL", "nl_NL"},
            {"nl-BE", "nl_NL"},

            {"pl-PL", "pl_PL"},

            {"pt-BR", "pt_BR"},
            {"pt-PT", "pt_PT"},

            {"ru-RU", "ru_RU"},

            {"sk-SK", "sk_SK"},

            {"sv-SE", "sv_SE"},
            {"sv-SV", "sv_SE"},

            {"tr-TR", "tr_TR"},

            {"zh-CN", "zh_TW"},
            {"zh-HK", "zh_TW"},
            {"zh-SG", "zh_TW"},
            {"zh-TW", "zh_TW"},
            {"zh-CHT", "zh_TW"},
            {"zh-HANS", "zh_TW"},
            {"zh-HANT", "zh_TW"}
        };

        static List<string> OffsetNames = new List<string>
        {
            "HEAD", "HELMET",
            "BODY", "CHEST", "BELT",
            "ARM0", "ARMARMOR0", "SHOULDER0", "TOOL0",
            "ARM1", "ARMARMOR1", "SHOULDER1", "TOOL1",
            "LEG0", "LEGGING0", "BOOT0",
            "LEG1", "LEGGING1", "BOOT1"
        };

        static string[,] ItemSheetArray =
        {
        {"leather_helmet","chainmail_helmet","iron_helmet","diamond_helmet","golden_helmet","flint_and_steel","flint","coal","string","wheat_seeds","apple","golden_apple","egg","sugar","snowball","elytra" },
        {"leather_chestplate","chainmail_chestplate","iron_chestplate","diamond_chestplate","golden_chestplate","bow","brick","iron_ingot","feather","wheat","painting","sugarcane","bone","cake","slime_ball","broken_elytra" },
        {"leather_leggings","chainmail_leggings","iron_leggings","diamond_leggings","golden_leggings","arrow","end_crystal","gold_ingot","gunpowder","bread","oak_sign","oak_door","iron_door","","fire_charge","chorus_fruit" },
        {"leather_boots","chainmail_boots","iron_boots","diamond_boots","golden_boots","stick","compass_00","diamond","redstone","clay_ball","paper","book","map","pumpkin_seeds","melon_seeds","popped_chorus_fruit" },
        {"wooden_sword","stone_sword","iron_sword","diamond_sword","golden_sword","fishing_rod","clock_00","bowl","mushroom_stew","glowstone_dust","bucket","water_bucket","lava_bucket","milk_bucket","ink_sac","gray_dye" },
        {"wooden_shovel","stone_shovel","iron_shovel","diamond_shovel","golden_shovel","fishing_rod_cast","repeater","porkchop","cooked_porkchop","cod","cooked_cod","rotten_flesh","cookie","shears","red_dye","pink_dye" },
        {"wooden_pickaxe","stone_pickaxe","iron_pickaxe","diamond_pickaxe","golden_pickaxe","bow_pulling_0","carrot_on_a_stick","leather","saddle","beef","cooked_beef","ender_pearl","blaze_rod","melon_slice","green_dye","lime_dye" },
        {"wooden_axe","stone_axe","iron_axe","diamond_axe","golden_axe","bow_pulling_1","baked_potato","potato","carrot","chicken","cooked_chicken","ghast_tear","gold_nugget","nether_wart","cocoa_beans","yellow_dye" },
        {"wooden_hoe","stone_hoe","iron_hoe","diamond_hoe","golden_hoe","bow_pulling_2","poisonous_potato","minecart","oak_boat","glistering_melon_slice","fermented_spider_eye","spider_eye","potion","potion_overlay","blue_dye","light_blue_dye" },
        {"leather_helmet_overlay","spectral_arrow","iron_horse_armor","diamond_horse_armor","golden_horse_armor","comparator","golden_carrot","chest_minecart","pumpkin_pie","spawn_egg","splash_potion","ender_eye","cauldron","blaze_powder","purple_dye","magenta_dye" },
        {"","tipped_arrow_base","dragon_breath","name_tag","lead","nether_brick","tropical_fish","furnace_minecart","charcoal","spawn_egg_overlay","","experience_bottle","brewing_stand","magma_cream","cyan_dye","orange_dye" },
        {"leather_leggings_overlay","tipped_arrow_head","lingering_potion","barrier","mutton","rabbit","pufferfish","hopper_minecart","hopper","nether_star","emerald","writable_book","written_book","flower_pot","light_gray_dye","bone_meal" },
        {"leather_boots_overlay","beetroot","beetroot_seeds","beetroot_soup","cooked_mutton","cooked_rabbit","salmon","tnt_minecart","armor_stand","firework_rocket","firework_star","firework_star_overlay","quartz","map","item_frame","enchanted_book" },
        {"acacia_door","birch_door","dark_oak_door","jungle_door","spruce_door","rabbit_stew","cooked_salmon","command_block_minecart","acacia_boat","birch_boat","dark_oak_boat","jungle_boat","spruce_boat","prismarine_shard","prismarine_crystals","leather_horse_armor" },
        {"structure_void","","totem_of_undying","shulker_shell","iron_nugget","rabbit_foot","rabbit_hide","","","","","","","","","" },
        {"music_disc_13","music_disc_cat","music_disc_blocks","music_disc_chirp","music_disc_far","music_disc_mall","music_disc_mellohi","music_disc_stal","music_disc_strad","music_disc_ward","music_disc_11","music_disc_wait","cod_bucket","salmon_bucket","pufferfish_bucket","tropical_fish_bucket" },
        {"leather_horse_armor","","","","","","","kelp","dried_kelp","sea_pickle","nautilus_shell","heart_of_the_sea","turtle_helmet","scute","trident","phantom_membrane" }
        };

        static string[,] BlockSheetArray =
        {
            {"grass_block_top","stone","dirt","grass_block_side","oak_planks","smooth_stone_slab_side","smooth_stone","bricks","tnt_side","tnt_top","tnt_bottom","cobweb","poppy","dandelion","blue_concrete","oak_sapling" },
            {"cobblestone","bedrock","sand","gravel","oak_log","oak_log_top","iron_block","gold_block","diamond_block","emerald_block","redstone_block","dropper_front","red_mushroom","brown_mushroom","jungle_sapling","red_concrete" },
            {"gold_ore","iron_ore","coal_ore","bookshelf","mossy_cobblestone","obsidian","grass_block_side_overlay","grass","dispenser_front_vertical","beacon","dropper_front_vertical","crafting_table_top","furnace_front","furnace_side","dispenser_front","red_concrete" },
            {"sponge","glass","diamond_ore","redstone_ore","oak_leaves","black_concrete","stone_bricks","dead_bush","fern","daylight_detector_top","daylight_detector_side","crafting_table_side","crafting_table_front","furnace_front_on","furnace_top","spruce_sapling" },
            {"white_wool","spawner","snow","ice","grass_block_snow","cactus_top","cactus_side","cactus_bottom","clay","sugar_cane","jukebox_side","jukebox_top","birch_leaves","mycelium_side","mycelium_top","birch_sapling" },
            {"torch","oak_door_top","iron_door_top","ladder","oak_trapdoor","iron_bars","farmland_wet","farmland","wheat_stage0","wheat_stage1","wheat_stage2","wheat_stage3","wheat_stage4","wheat_stage5","wheat_stage6","wheat_stage7" },
            {"lever","oak_door_bottom","iron_door_bottom","redstone_torch","mossy_stone_bricks","cracked_stone_bricks","pumpkin_top","netherrack","soul_sand","glowstone","piston_top_sticky","piston_top","piston_side","piston_bottom","piston_inner","pumpkin_stem" },
            {"rail_corner","black_wool","gray_wool","redstone_torch_off","spruce_log","birch_log","pumpkin_side","carved_pumpkin","jack_o_lantern","cake_top","cake_side","cake_inner","cake_bottom","red_mushroom_block","brown_mushroom_block","attached_pumpkin_stem" },
            {"rail","red_wool", "pink_wool","repeater","spruce_leaves","spruce_leaves","conduit","turtle_egg","melon_side","melon_top","cauldron_top","cauldron_inner","wet_sponge","mushroom_stem","mushroom_block_inside","vines" },
            {"lapis_block","green_wool","lime_wool","repeater_on","glass_pane_top","debug","debug","turtle_egg_slightly_cracked","turtle_egg_very_cracked","jungle_log","cauldron_side","cauldron_bottom","brewing_stand_base","brewing_stand","end_portal_frame_top","end_portal_frame_side" },
            {"lapis_ore","brown_wool","yellow_wool","powered_rail","redstone_dust_dot","redstone_dust_line0","enchanting_table_top","dragon_egg","cocoa_stage2","cocoa_stage1","cocoa_stage0","emerald_ore","tripwire_hook","tripwire","end_portal_frame_eye","end_stone" },
            {"sandstone_top","blue_wool","light_blue_wool","powered_rail_on","debug","debug","enchanting_table_side","enchanting_table_bottom","glide_blue","item_frame","flower_pot","comparator","comparator_on","activator_rail","activator_rail","nether_quartz_ore" },
            {"sandstone","purple_wool","magenta_wool","detector_rail","jungle_leaves","black_concrete","spruce_planks","jungle_planks","carrots_stage0","carrots_stage1","carrots_stage2","carrots_stage3","slime_block","debug","debug","debug" },
            {"sandstone_bottom","cyan_wool","orange_wool","redstone_lamp","redstone_lamp_on","chiseled_stone_bricks","birch_planks","anvil","chipped_anvil_top","chiseled_quartz_block_top","quartz_pillar_top","quartz_block_side","debug","detector_rail_on","debug","debug" },
            {"nether_bricks","light_gray_wool","nether_wart_stage0","nether_wart_stage1","nether_wart_stage2","chiseled_sandstone","cut_sandstone","anvil_top","damaged_anvil_top","chiseled_quartz_block","quartz_pillar","quartz_block_top","debug","debug","debug","debug" },
            {"destroy_stage_0","destroy_stage_1","destroy_stage_2","destroy_stage_3","destroy_stage_4","destroy_stage_5","destroy_stage_6","destroy_stage_7","destroy_stage_8","destroy_stage_9","hay_block_side","quartz_block_bottom","debug","hay_block_top","debug","debug" },
            {"coal_block","terracotta","note_block","andesite","polished_andesite","diorite","polished_diorite","granite","polished_granite","potatoes_stage0","potatoes_stage1","potatoes_stage2","potatoes_stage3","spruce_log_top","jungle_log_top","birch_log_top" },
            {"black_terracotta","blue_terracotta","brown_terracotta","cyan_terracotta","gray_terracotta","green_terracotta","light_blue_terracotta","lime_terracotta","magenta_terracotta","orange_terracotta","pink_terracotta","purple_terracotta","red_terracotta","light_gray_terracotta","white_terracotta","yellow_terracotta" },
            {"black_stained_glass","blue_stained_glass","brown_stained_glass","cyan_stained_glass","gray_stained_glass","green_stained_glass","light_blue_stained_glass","lime_stained_glass","magenta_stained_glass","orange_stained_glass","pink_stained_glass","purple_stained_glass","red_stained_glass","light_gray_stained_glass","white_stained_glass","yellow_stained_glass" },
            {"black_stained_glass_pane_top","blue_stained_glass_pane_top","brown_stained_glass_pane_top","cyan_stained_glass_pane_top","gray_stained_glass_pane_top","green_stained_glass_pane_top","light_blue_stained_glass_pane_top","lime_stained_glass_pane_top","magenta_stained_glass_pane_top","orange_stained_glass_pane_top","pink_stained_glass_pane_top","purple_stained_glass_pane_top","red_stained_glass_pane_top","light_gray_stained_glass_pane_top","white_stained_glass_pane_top","yellow_stained_glass_pane_top" },
            {"large_fern_top","tall_grass_top","peony_top","rose_bush_top","lilac_top","orange_tulip","sunflower_top","sunflower_front","acacia_log","acacia_log_top","acacia_planks","acacia_leaves","acacia_leaves","prismarine_bricks","red_sand","red_sandstone_top" },
            {"large_fern_bottom","tall_grass_bottom","peony_bottom","rose_bush_bottom","lilac_bottom","pink_tulip","sunflower_bottom","sunflower_back","dark_oak_log","dark_oak_log_top","dark_oak_planks","dark_oak_leaves","dark_oak_leaves","dark_prismarine","red_sandstone_bottom","red_sandstone" },
            {"allium","blue_orchid","azure_bluet","oxeye_daisy","red_tulip","white_tulip","acacia_sapling","dark_oak_sapling","coarse_dirt","podzol_side","podzol_top","spruce_leaves","spruce_leaves","debug","chiseled_red_sandstone","cut_red_sandstone" },
            {"acacia_door_top","birch_door_top","dark_oak_door_top","jungle_door_top","spruce_door_top","chorus_flower","chorus_flower_dead","chorus_plant","end_stone_bricks","grass_path_side","grass_path_top","debug","packed_ice","debug","daylight_detector_inverted_top","iron_trapdoor" },
            {"acacia_door_bottom","birch_door_bottom","dark_oak_door_bottom","jungle_door_bottom","spruce_door_bottom","purpur_block","purpur_pillar","purpur_pillar_top","end_rod","debug","nether_wart_block","red_nether_bricks","frosted_ice_0","frosted_ice_1","frosted_ice_2","frosted_ice_3" },
            {"beetroots_stage0","beetroots_stage1","beetroots_stage2","beetroots_stage3","debug","debug","debug","debug","debug","debug","debug","debug","debug","debug","debug","debug" },
            {"bone_block_side","bone_block_top","melon_stem","attached_melon_stem","observer_front","observer_side","observer_back","observer_back_on","observer_top","glide_yellow","glide_green","structure_block","structure_block_corner","structure_block_data","structure_block_load","structure_block_save" },
            {"black_concrete","blue_concrete","brown_concrete","cyan_concrete","gray_concrete","green_concrete","light_blue_concrete","lime_concrete","magenta_concrete","orange_concrete","pink_concrete","purple_concrete","red_concrete","light_gray_concrete","white_concrete","yellow_concrete" },
            {"black_concrete_powder","blue_concrete_powder","brown_concrete_powder","cyan_concrete_powder","gray_concrete_powder","green_concrete_powder","light_blue_concrete_powder","lime_concrete_powder","magenta_concrete_powder","orange_concrete_powder","pink_concrete_powder","purple_concrete_powder","red_concrete_powder","light_gray_concrete_powder","white_concrete_powder","yellow_concrete_powder" },
            {"black_glazed_terracotta","blue_glazed_terracotta","brown_glazed_terracotta","cyan_glazed_terracotta","gray_glazed_terracotta","green_glazed_terracotta","light_blue_glazed_terracotta","lime_glazed_terracotta","magenta_glazed_terracotta","orange_glazed_terracotta","pink_glazed_terracotta","purple_glazed_terracotta","red_glazed_terracotta","light_gray_glazed_terracotta","white_glazed_terracotta","yellow_glazed_terracotta" },
            {"white_shulker_box","","water_overlay","debug","tube_coral_block","bubble_coral_block","brain_coral_block","fire_coral_block","horn_coral_block","tube_coral","bubble_coral","brain_coral","fire_coral","horn_coral","sea_pickle","blue_ice" },
            {"dried_kelp_top","dried_kelp_side","debug","debug","dead_tube_coral_block","dead_bubble_coral_block","dead_brain_coral_block","dead_fire_coral_block","dead_horn_coral_block","tube_coral_fan","bubble_coral_fan","brain_coral_fan","fire_coral_fan","horn_coral_fan","","" },
            {"debug","debug","debug","debug","debug","debug","debug","debug","debug","dead_tube_coral_fan","dead_bubble_coral_fan","dead_brain_coral_fan","dead_fire_coral_fan","dead_horn_coral_fan","","spruce_trapdoor" },
            {"stripped_oak_log","stripped_oak_log_top","stripped_acacia_log","stripped_acacia_log_top","stripped_birch_log","stripped_birch_log_top","stripped_dark_oak_log","stripped_dark_oak_log_top","stripped_jungle_log","stripped_jungle_log_top","stripped_spruce_log","stripped_spruce_log_top","acacia_trapdoor","birch_trapdoor","dark_oak_trapdoor","jungle_trapdoor" }
        };

        static Dictionary<string, string> ImgCopyLookup = new Dictionary<string, string>()
        {
            { "res/mob", "/textures/entity" },
            { "res/art", "/textures/painting" },
            { "res/environment", "/textures/environment" },
            { "res/terrain", "/textures/environment" },
            { "res/armor", "/textures/models/armor" }
        };

        #region Texture Packs

        public void ConvertTexturePack(PckFile sourcePck, string exportPath)
        {
            Directory.CreateDirectory(exportPath);
            foreach (PckFile.FileData file in sourcePck.Files)
            {
                switch (file.Filetype)
                {
                    case PckFile.FileData.FileType.TextureFile:
                        if (file.Filename == "res/terrain.png")
                            SplitSheet(file.Data, exportPath, useItemsArray: false);
                        if (file.Filename == "res/items.png")
                            SplitSheet(file.Data, exportPath, useItemsArray: true);
                        break;
                    case PckFile.FileData.FileType.UIDataFile:
                        CopyTexture(file, exportPath);
                        break;
                }
            }
        }

        void SplitSheet(byte[] data, string exportPath, bool useItemsArray)
        {
            int defaultHorizontalCount = 16;
            int defaultVerticalCount = 34;
            string[,] sheetArray = BlockSheetArray;
            string outputPath = "\\textures\\blocks\\";
            if (useItemsArray)
            {
                defaultVerticalCount = 17; 
                sheetArray = ItemSheetArray;
                outputPath = "\\textures\\items\\";
            }
            Directory.CreateDirectory(exportPath + outputPath);

            MemoryStream ms = new MemoryStream(data);
            var img = Image.FromStream(ms);

            int targetWidth = img.Width / defaultHorizontalCount;
            int targetHeight = img.Height / defaultVerticalCount;

            // Start splitting the Image.
            Bitmap piece = new Bitmap(targetWidth, targetHeight);
            Rectangle dest_rect = new Rectangle(0, 0, targetWidth, targetHeight);
            using (Graphics gr = Graphics.FromImage(piece))
            {
                int num_rows = img.Height / targetHeight;
                int num_cols = img.Width / targetWidth;
                Rectangle source_rect = new Rectangle(0, 0, targetWidth, targetHeight);
                for (int row = 0; row < num_rows; row++)
                {
                    source_rect.X = 0;
                    for (int col = 0; col < num_cols; col++)
                    {
                        // Copy the piece of the image.
                        gr.Clear(Color.Transparent);
                        gr.DrawImage(img, dest_rect, source_rect, GraphicsUnit.Pixel);

                        // Save the piece.
                        string filename = sheetArray[row, col] + ".png";
                        if(!string.IsNullOrEmpty(filename) && filename != "debug")
                            piece.Save(exportPath + outputPath + filename, System.Drawing.Imaging.ImageFormat.Png);

                        // Move to the next column.
                        source_rect.X += targetWidth;
                    }
                    source_rect.Y += targetHeight;
                }
            }
        }

        [Obsolete("Wrong Behavior ?")]
        void CopyTexture(PckFile.FileData file, string exportPath)
        {
            string exportName = file.Filename;
            foreach(var kvp in ImgCopyLookup)
            {
                exportName = exportName.Replace(kvp.Key, kvp.Value);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(exportPath + exportName));
            File.WriteAllBytes(exportPath + exportName, file.Data);
        }

        #endregion

        #region Skin Packs
        public void ConvertSkinPack(PckFile sourcePck, string exportFilepath)
        {
            List<string> localisables = new List<string>();

            SkinJSON SJSON = new SkinJSON(); // Skins.json

            string ExportPath = Path.GetDirectoryName(exportFilepath);
            Directory.CreateDirectory(Path.Combine(ExportPath, "skin_pack"));

            if (sourcePck.TryGetFile("localisation.loc", PckFile.FileData.FileType.LocalisationFile, out PckFile.FileData locFileData) ||
                sourcePck.TryGetFile("languages.loc", PckFile.FileData.FileType.LocalisationFile, out locFileData))
            {
                var reader = new LOCFileReader();

                using (var ms = new MemoryStream(locFileData.Data))
                {
                    LOCFile locFile = reader.FromStream(ms);
                    string newPackName = locFile.GetLocEntry("IDS_DISPLAY_NAME", "en-EN").ToLower().Replace(":", "");
                    if (!string.IsNullOrEmpty(newPackName))
                    {
                        PackName = newPackName.Replace(" ", "_");
                    }

                    localisables.Add(PackName);
                    ExportLOC(locFile, ExportPath + "\\skin_pack\\texts");
                }
            }

            SJSON.skins = ConvertSkins(sourcePck, ExportPath).ToArray();
            SJSON.localization_name = SJSON.serialize_name = localisables[0];

            CreateSkinPackManifest(ExportPath + "\\skin_pack", localisables[0]);

            string SKINS_JSON = JsonConvert.SerializeObject(SJSON, Formatting.Indented);
            File.WriteAllText(ExportPath + "\\skin_pack\\skins.json", SKINS_JSON);

            string GEO_JSON = JsonConvert.SerializeObject(GJSON, Formatting.Indented);
            File.WriteAllText(ExportPath + "\\skin_pack\\geometry.json", GEO_JSON);

            using (var outputStream = new ZipOutputStream(File.Create(exportFilepath)))
            {
                outputStream.SetLevel(Deflater.NO_COMPRESSION);
                string[] files = Directory.GetFiles(ExportPath + "\\skin_pack", "*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    ZipEntry entry = new ZipEntry(file.Replace(ExportPath + "\\", ""));
                    outputStream.PutNextEntry(entry);

                    byte[] sourceBytes = File.ReadAllBytes(file);
                    outputStream.Write(sourceBytes, 0, sourceBytes.Length);
                }
                //Directory.Delete(ExportPath + "\\skin_pack", true);
            }
            GC.Collect();
        }

        private List<SkinObject> ConvertSkins(PckFile sourcePck, string exportPath)
        {
            List<SkinObject> skins = new List<SkinObject>();
            foreach (PckFile.FileData file in sourcePck.Files)
            {
                switch (file.Filetype)
                {
                    case PckFile.FileData.FileType.SkinFile:
                        skins.Add(ExportSkin(file, exportPath + "\\skin_pack"));
                        break;
                    case PckFile.FileData.FileType.CapeFile:
                        ExportCape(file, exportPath + "\\skin_pack");
                        break;
                    case PckFile.FileData.FileType.SkinDataFile:
                        var reader = new PckFileReader();
                        using (var ms = new MemoryStream(file.Data))
                        {
                            PckFile subPack = reader.FromStream(ms);
                            skins.AddRange(ConvertSkins(subPack, exportPath));
                        }
                        break;
                }
            }
            return skins;
        }

        void ExportLOC(LOCFile locFile, string exportPath)
        {
            Directory.CreateDirectory(exportPath);
            List<string> languages = new List<string>();

            foreach (var kvp in locFile.LocKeys)
            {
                string packString = kvp.Key.Replace("IDS_DISPLAY_NAME", "skinpack." + PackName).Replace("IDS_DLCSKIN", "skin." + PackName + ".").Replace("_DISPLAYNAME", "").Replace(" ", "");

                if (!packString.EndsWith("_THEMENAME"))
                {
                    foreach (KeyValuePair<string, string> text in kvp.Value)
                    {
                        string bedrockLang = languageMap[text.Key];
                        if (!languages.Contains(bedrockLang))
                            languages.Add(bedrockLang);
                        if (File.Exists(exportPath + "\\" + bedrockLang + ".lang"))
                        {
                            if (!File.ReadAllText(exportPath + "\\" + bedrockLang + ".lang").Contains(packString))
                            {
                                using (StreamWriter sw = new StreamWriter(exportPath + "\\" + bedrockLang + ".lang", true))
                                {
                                    sw.WriteLine(packString + "=" + text.Value);
                                }
                            }
                        }
                        else
                        {
                            using (StreamWriter sw = new StreamWriter(exportPath + "\\" + bedrockLang + ".lang", true))
                            {
                                sw.WriteLine(packString + "=" + text.Value);
                            }
                        }
                    }
                }
            }

            string serializedLanguages = JsonConvert.SerializeObject(languages.ToArray(), Formatting.Indented);
            File.WriteAllText(exportPath + "\\languages.json", serializedLanguages);
        }

        List<(string, string)> offsets = null;

        List<(string, string)> GetSkinOffsets(PckFile.PCKProperties skinProperties)
        {
            List<(string, string)> skinOffsets = new List<ValueTuple<string, string>>();

            string part_offset = "";

            foreach (string offsetName in OffsetNames)
            {
                try
                {
                    var v = skinProperties.Find(prop => prop.property == "OFFSET" && prop.value.StartsWith(offsetName)).value;
                    if (v != null && v.Length >= 2) part_offset = v.Split(' ')[2];
                    else part_offset = "0";
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex);
                    part_offset = "0";
                }

                switch (offsetName)
				{
                    case "HEAD":
                        skinOffsets.Add(("HEADWEAR", part_offset));
                        break;
                    case "BODY":
                        skinOffsets.Add(("JACKET", part_offset));
                        skinOffsets.Add(("BODYARMOR", part_offset));
                        break;
                    case "CHEST":
                        skinOffsets.Add(("BELT", part_offset));
                        skinOffsets.Add(("WAIST", part_offset));
                        break;
                    case "ARM0":
                        skinOffsets.Add(("SLEEVE0", part_offset));
                        skinOffsets.Add(("SHOULDER0", part_offset));
                        break;
                    case "ARM1":
                        skinOffsets.Add(("SLEEVE1", part_offset));
                        skinOffsets.Add(("SHOULDER1", part_offset));
                        break;
                    case "LEG0":
                        skinOffsets.Add(("PANTS0", part_offset));
                        skinOffsets.Add(("SOCK0", part_offset));
                        break;
                    case "LEG1":
                        skinOffsets.Add(("PANTS1", part_offset));
                        skinOffsets.Add(("SOCK1", part_offset));
                        break;
                }
                
                if(skinOffsets.Find(p => p.Item1 == offsetName) != default!)
                    skinOffsets.Add(new ValueTuple<string, string>(offsetName, part_offset));
            }

            return skinOffsets;
        }

        modelCube[] ConvertBoxes(string part, PckFile.FileData file, Vector3 pivot)
        {
            List<modelCube> cubes = new List<modelCube>();

            var anim = new SkinANIM();

            Console.WriteLine(part);
            float offset = float.Parse(offsets.Find(o => o.Item1 == part).Item2);

            foreach (var (name, value) in file.Properties)
            {
                string entry = value;
                switch (name)
                {
                    case "ANIM":
                        anim = SkinANIM.FromString(value);
                        break;
                    case "BOX":
                        SkinBOX box = SkinBOX.FromString(entry);
                        if (box.Parent == part)
                        {
                            float y = -1 * (box.Pos.Y + offset + box.Size.Y);
                            cubes.Add(new modelCube(new Vector3(pivot.X + box.Pos.X, pivot.Y + y, pivot.Z + box.Pos.Z),
                                box.Size, box.UV, box.Mirror, box.Inflation));
                        }
                        break;
                    default:
                        break;
                }
            }

            bool slim = anim.GetFlag(ANIM_EFFECTS.SLIM_MODEL);
            bool classic_res = !(slim && anim.GetFlag(ANIM_EFFECTS.RESOLUTION_64x64));

            switch (part)
            {
                case "HEAD":
                    if (!anim.GetFlag(ANIM_EFFECTS.HEAD_DISABLED))
                        cubes.Add(new modelCube(new Vector3(- 4, 24 - offset, -4), new Vector3(8, 8, 8), new Vector2(0, 0)));
                    break;
                case "BODY":
                    if (!anim.GetFlag(ANIM_EFFECTS.BODY_DISABLED))
                        cubes.Add(new modelCube(new Vector3(-4, 12 - offset, -2), new Vector3(8, 12, 4), new Vector2(16, 16)));
                    break;
                case "ARM0":
                    if (!anim.GetFlag(ANIM_EFFECTS.RIGHT_ARM_DISABLED))
                        cubes.Add(new modelCube(new Vector3(slim ? -7 : - 8, 12 - offset, -2), new Vector3(slim ? 3 : 4, 12, 4), new Vector2(40, 16)));
                    break;
                case "ARM1":
                    if (!anim.GetFlag(ANIM_EFFECTS.LEFT_ARM_DISABLED))
                        cubes.Add(new modelCube(new Vector3(4, 12 - offset, -2), new Vector3(slim ? 3 : 4, 12, 4), classic_res ? new Vector2(40, 16) : new Vector2(32, 48), classic_res));
                    break;
                case "LEG0":
                    if (!anim.GetFlag(ANIM_EFFECTS.RIGHT_LEG_DISABLED))
                        cubes.Add(new modelCube(new Vector3(-3.9f, 0 - offset, -2), new Vector3(4, 12, 4), new Vector2(0, 16)));
                    break;
                case "LEG1":
                    if (!anim.GetFlag(ANIM_EFFECTS.LEFT_LEG_DISABLED))
                        cubes.Add(new modelCube(new Vector3(0.1f, 0 - offset, -2), new Vector3(4, 12, 4), classic_res ? new Vector2(0, 16) : new Vector2(16, 48), classic_res));
                    break;
                case "HEADWEAR":
                    if (!anim.GetFlag(ANIM_EFFECTS.HEAD_OVERLAY_DISABLED))
                        cubes.Add(new modelCube(new Vector3(-4, 24 - offset, -4), new Vector3(8, 8, 8), new Vector2(32, 0), false, 0.5f));
                    break;
                case "JACKET":
                    if (!classic_res && !anim.GetFlag(ANIM_EFFECTS.BODY_OVERLAY_DISABLED))
                        cubes.Add(new modelCube(new Vector3(0, 24 - offset, 0), new Vector3(8, 12, 4), new Vector2(16, 32), false, 0.25f));
                    break;
                case "SLEEVE0":
                    if (!classic_res && !anim.GetFlag(ANIM_EFFECTS.RIGHT_ARM_OVERLAY_DISABLED))
                        cubes.Add(new modelCube(new Vector3(slim ? -7 : -8, 12 - offset, -2), new Vector3(slim ? 3 : 4, 12, 4), new Vector2(40, 32), false, 0.25f));
                    break;
                case "SLEEVE1":
                    if (!classic_res && !anim.GetFlag(ANIM_EFFECTS.LEFT_ARM_OVERLAY_DISABLED))
                        cubes.Add(new modelCube(new Vector3(4, 12 - offset, -2), new Vector3(slim ? 3 : 4, 12, 4), new Vector2(48, 48), false, 0.25f));
                    break;
                case "PANTS0":
                    if (!classic_res && !anim.GetFlag(ANIM_EFFECTS.RIGHT_LEG_OVERLAY_DISABLED))
                        cubes.Add(new modelCube(new Vector3(-3.9f, 0 - offset, -2), new Vector3(4, 12, 4), new Vector2(0, 32), false, 0.25f));
                    break;
                case "PANTS1":
                    if (!classic_res && !anim.GetFlag(ANIM_EFFECTS.LEFT_LEG_OVERLAY_DISABLED))
                        cubes.Add(new modelCube(new Vector3(0.1f, 0 - offset, -2), new Vector3(4, 12, 4), new Vector2(0, 48), false, 0.25f));
                    break;
                default:
                    break;
            }

            return cubes.ToArray();
        }

        SkinObject ExportSkin(PckFile.FileData file, string exportPath)
        {
            if (file.Filetype != PckFile.FileData.FileType.SkinFile)
                return default!;

            SkinObject skinObj = new SkinObject();
            string skinID = file.Filename.Replace(".png", "").Replace("Skins/", "");
            skinObj.localization_name = skinID;
            skinObj.texture = skinID + ".png";

            skinObj.geometry = "geometry." + PackName + "." + skinID;

            Vector3 head_and_body_pivot = new Vector3(0, 24, 0);
            Vector3 right_arm_pivot = new Vector3(-5, 22, 0);
            Vector3 left_arm_pivot = new Vector3(5, 22, 0);
            Vector3 right_leg_pivot = new Vector3(-1.9f, 12, 0);
            Vector3 left_leg_pivot = new Vector3(1.9f, 12, 0);

            offsets = GetSkinOffsets(file.Properties);

            List<modelBone> bones = new List<modelBone>();

            bones.Add(new modelBone("head", null, head_and_body_pivot, ConvertBoxes("HEAD", file, head_and_body_pivot), "base"));
            bones.Add(new modelBone("body", null, head_and_body_pivot, ConvertBoxes("BODY", file, head_and_body_pivot), "base"));
            bones.Add(new modelBone("rightArm", null, right_arm_pivot, ConvertBoxes("ARM0", file, right_arm_pivot), "base"));
            bones.Add(new modelBone("leftArm", null, left_arm_pivot, ConvertBoxes("ARM1", file, left_arm_pivot), "base"));
            bones.Add(new modelBone("rightLeg", null, right_leg_pivot, ConvertBoxes("LEG0", file, right_leg_pivot), "base"));
            bones.Add(new modelBone("leftLeg", null, left_leg_pivot, ConvertBoxes("LEG1", file, left_leg_pivot), "base"));

            bones.Add(new modelBone("hat", "head", head_and_body_pivot, ConvertBoxes("HEADWEAR", file, head_and_body_pivot), "clothing"));
            bones.Add(new modelBone("jacket", "body", head_and_body_pivot, ConvertBoxes("JACKET", file, head_and_body_pivot), "clothing"));
            bones.Add(new modelBone("rightSleeve", "rightArm", right_arm_pivot, ConvertBoxes("SLEEVE0", file, right_arm_pivot), "clothing"));
            bones.Add(new modelBone("leftSleeve", "leftArm", left_arm_pivot, ConvertBoxes("SLEEVE1", file, left_arm_pivot), "clothing"));
            bones.Add(new modelBone("rightPants", "rightLeg", right_leg_pivot, ConvertBoxes("PANTS0", file, right_leg_pivot), "clothing"));
            bones.Add(new modelBone("leftPants", "leftLeg", left_leg_pivot, ConvertBoxes("PANTS1", file, left_leg_pivot), "clothing"));
            bones.Add(new modelBone("rightSock", "rightLeg", right_leg_pivot, ConvertBoxes("SOCK0", file, right_leg_pivot), "clothing"));
            bones.Add(new modelBone("leftSock", "leftLeg", left_leg_pivot, ConvertBoxes("SOCK1", file, left_leg_pivot), "clothing"));

            bones.Add(new modelBone("helmet", "head", head_and_body_pivot, ConvertBoxes("HELMET", file, head_and_body_pivot), "armor"));
            bones.Add(new modelBone("bodyArmor", "body", head_and_body_pivot, ConvertBoxes("BODYARMOR", file, head_and_body_pivot), "armor"));
            bones.Add(new modelBone("belt", "body", head_and_body_pivot, ConvertBoxes("BELT", file, head_and_body_pivot), "armor"));
            bones.Add(new modelBone("rightArmArmor", "rightArm", right_arm_pivot, ConvertBoxes("ARMARMOR0", file, right_arm_pivot), "armor"));
            bones.Add(new modelBone("leftArmArmor", "leftArm", left_arm_pivot, ConvertBoxes("ARMARMOR1", file, left_arm_pivot), "armor"));
            bones.Add(new modelBone("rightLegArmor", "rightLeg", right_leg_pivot, ConvertBoxes("LEGGING0", file, right_leg_pivot), "armor"));
            bones.Add(new modelBone("leftLegArmor", "leftLeg", left_leg_pivot, ConvertBoxes("LEGGING1", file, left_leg_pivot), "armor"));
            bones.Add(new modelBone("rightBoot", "rightLeg", right_leg_pivot, ConvertBoxes("BOOT0", file, right_leg_pivot), "armor"));
            bones.Add(new modelBone("leftBoot", "leftLeg", left_leg_pivot, ConvertBoxes("BOOT1", file, left_leg_pivot), "armor"));

            // calculates armor and item offsets
            bones.Add(new modelBone("rightItem", "rightArm", new Vector3(-6.0f, 15.0f - float.Parse(offsets.Find(o => o.Item1 == "TOOL0").Item2), 1.0f), null, "item"));
            bones.Add(new modelBone("leftItem", "leftArm", new Vector3(6.0f, 15.0f - float.Parse(offsets.Find(o => o.Item1 == "TOOL1").Item2), 1.0f), null, "item"));

            bones.Add(new modelBone("helmetArmorOffset", "head", new Vector3(0f, 24f - float.Parse(offsets.Find(o => o.Item1 == "HEAD").Item2), 0f), null, "armor_offset"));
            bones.Add(new modelBone("bodyArmorOffset", "body", new Vector3(- 4f, 12f - float.Parse(offsets.Find(o => o.Item1 == "BODY").Item2), -2f), null, "armor_offset"));
            bones.Add(new modelBone("rightArmArmorOffset", "rightArm", new Vector3(4f, 12f - float.Parse(offsets.Find(o => o.Item1 == "ARM0").Item2), -2f), null, "armor_offset"));
            bones.Add(new modelBone("leftArmArmorOffset", "leftArm", new Vector3(- 8f, 12f - float.Parse(offsets.Find(o => o.Item1 == "ARM1").Item2), -2f), null, "armor_offset"));
            bones.Add(new modelBone("rightLegArmorOffset", "rightLeg", new Vector3(-0.1f, float.Parse(offsets.Find(o => o.Item1 == "LEG0").Item2), -2f), null, "armor_offset"));
            bones.Add(new modelBone("leftLegArmorOffset", "leftLeg", new Vector3(-4.1f, float.Parse(offsets.Find(o => o.Item1 == "LEG1").Item2), -2f), null, "armor_offset"));
            bones.Add(new modelBone("rightBootArmorOffset", "rightLeg", new Vector3(2f, 12f - float.Parse(offsets.Find(o => o.Item1 == "BOOT0").Item2), 0f), null, "armor_offset"));
            bones.Add(new modelBone("leftBootArmorOffset", "leftLeg", new Vector3(- 2f, 12f - float.Parse(offsets.Find(o => o.Item1 == "BOOT1").Item2), 0f), null, "armor_offset"));

            string capepath = file.Properties.Find(o => o.property == "CAPEPATH").value;

            JToken geo = JToken.FromObject(new skinModel(bones.ToArray()));
            if (capepath != null)
                geo["cape"] = capepath;

            GJSON.Add(skinObj.geometry, geo);

            File.WriteAllBytes(exportPath + "\\" + skinID + ".png", file.Data);
            return skinObj;
        }

        void ExportCape(PckFile.FileData file, string exportPath)
		{
            if (file.Filetype != PckFile.FileData.FileType.CapeFile)
                return;

            string capeID = file.Filename.Replace(".png", string.Empty).Replace("Skins/", string.Empty);
            File.WriteAllBytes(exportPath + "\\" + capeID + ".png", file.Data);
        }

        void CreateSkinPackManifest(string exportPath, string localizedName)
        {
            SkinManifest manifest = new SkinManifest();
            manifest.header = new Header()
            { 
                name = localizedName,
                description = "Exported with PCK Studio",
            };
            manifest.modules = new Module[] { new Module() };
            manifest.format_version = 1;
            string JSON = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            File.WriteAllText(exportPath + "\\manifest.json", JSON);
        }

        #endregion

        #region JSONObjects
        class SkinManifest
        {
            public Header header { get; set; }
            public Module[] modules { get; set; }
            public int format_version = 1;
        }

        class Header
        {
            public int[] version = { 1, 0, 0 };
            public string description = "";
            public string name = "";
            public string uuid = Guid.NewGuid().ToString();
        }

        class Module
        {
            public int[] version = { 1, 0, 0 };
            public string type = "skin_pack";
            public string uuid = Guid.NewGuid().ToString();
        }

        class SkinJSON
        {
            public SkinObject[] skins { get; set; }
            public string serialize_name = "";
            public string localization_name = "";
        }

        public class SkinObject
        {
            public string localization_name = "dlcskin00000000";
            public string geometry = "geometry.humanoid.custom";
            public string texture = "dlcskin00000000.png";
            public string type = "free";
        }

        #endregion

        #region Model classes for ANIM conversion

        public class modelCube
        {
            public modelCube(Vector3 origin, Vector3 size, Vector2 uv, bool mirror = false, float inflate = 0.0f)
            {
                this.origin = origin;
                this.size = size;
                this.uv = uv;
                this.mirror = mirror;
                this.inflate = inflate;
            }

            public Vector3 origin = Vector3.Zero;
            public Vector3 size = Vector3.Zero;
            // for whatever reason, uv is a float on LCE,
            // so I've kept it a float for the sake of consistency
            public Vector2 uv = Vector2.Zero;
            public bool mirror = false;
            public float inflate = 0.0f;
        }

        public class modelBone
        {
            public modelBone(string name, string parent, Vector3 pivot, modelCube[] cubes, string metaBone = "")
            {
                this.name = name;
                this.parent = parent;
                pivot.CopyTo(this.pivot);
                this.cubes = cubes;
                //if (!String.IsNullOrEmpty(metaBone)) this.META_BoneType = metaBone;
            }

            //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            //public string META_BoneType = null;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string name = "partName";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string parent = "parentName";

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public float[] pivot = { 0, 0, 0 };

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public modelCube[] cubes = null;

            public override string ToString()
			{
                return name + " - " + cubes.Length + " cubes";
			}
        }

        public class skinModel
        {
            public skinModel(modelBone[] bones)
            {
                this.bones = bones;
            }

            public int visible_bounds_width = 1;
            public int visible_bounds_height = 2;
            public int[] visible_bounds_offset = { 0, 1, 0};
            public modelBone[] bones;
        }
        #endregion
    }
}
