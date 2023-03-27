using System;
using System.Collections.Generic;
using System.IO;
using MapleLib.WzLib;
using UnityEngine;
using System.Xml;
using MapleLib.WzLib.WzProperties;
using System.Drawing;
using MapleLib;

namespace Maple
{
    public class WzResourceController
    {
        const string wzInfoXmlName_ = "WzInfo";

        const string wzMapleDictionary_ = "F:\\Projects\\Project.program\\UnityProject\\MapleFuture\\Data\\";

        const WzMapleVersion wzMapleVersion_ = WzMapleVersion.GMS;

        public static WzFileManager wzFileManager_ = null;

        public static WzInformationManager wzInfoManager_ = new WzInformationManager();

        /// <summary>
        /// Whole wz files loading
        /// </summary>
        public void Intialize()
        {
            // default GMS version
            InitializeWzFiles(wzMapleDictionary_, wzMapleVersion_);
        }

        private bool InitializeWzFiles(string directionary, WzMapleVersion version)
        {
            if(!Directory.Exists(directionary))
            {
                Logger.Report(LOGGER_LEVEL.LOGGER_LEVEL_INFO, "file path " + directionary + " not exist");
                return false;
            }

            if(wzFileManager_ != null)
            {
                wzFileManager_.Dispose();
                wzFileManager_ = null;
            }

            if(wzInfoManager_ != null)
            {
                wzInfoManager_.Clear();
            }

            wzFileManager_ = new WzFileManager(directionary, false);
            wzFileManager_.BuildWzFileList();

            // only for version beyond v30
            List<string> wzFileNames = GetWzBaseNamesFromXml();
            foreach (string wzFileName in wzFileNames)
            {
                var fileName = wzFileName.ToLower();
                List<string> wzSubFileList = wzFileManager_.GetWzFileNameListFromBase(fileName);
                foreach (string wzSubFile in wzSubFileList)
                {
                    Logger.Report(LOGGER_LEVEL.LOGGER_LEVEL_DEBUG, "initialize " + wzSubFile + " ... ...");
                    // List.wz cannot be loaded?
                    wzFileManager_.LoadWzFile(wzSubFile, version);
                }
            }

            ExtractStringWzMaps();
            ExtractMobFile();
            ExtractNpcFile();
            ExtractReactorFile();
            ExtractSoundFile();

            // load map related
            for (int i_map = 0; i_map <= 9; i_map++)
            {
                List<string> map_iWzFiles = wzFileManager_.GetWzFileNameListFromBase("map\\map\\map" + i_map);
                foreach (string map_iWzFileName in map_iWzFiles)
                {
                    wzFileManager_.LoadWzFile(map_iWzFileName, version);
                }
            }
            List<string> tileWzFiles = wzFileManager_.GetWzFileNameListFromBase("map\\tile"); // this doesnt exist before 64-bit client, and is kept in Map.wz
            foreach (string tileWzFileNames in tileWzFiles)
            {
                wzFileManager_.LoadWzFile(tileWzFileNames, version);
            }
            List<string> objWzFiles = wzFileManager_.GetWzFileNameListFromBase("map\\obj"); // this doesnt exist before 64-bit client, and is kept in Map.wz
            foreach (string objWzFileName in objWzFiles)
            {
                wzFileManager_.LoadWzFile(objWzFileName, version);
            }
            List<string> backWzFiles = wzFileManager_.GetWzFileNameListFromBase("map\\back"); // this doesnt exist before 64-bit client, and is kept in Map.wz
            foreach (string backWzFileName in backWzFiles)
            {
                wzFileManager_.LoadWzFile(backWzFileName, version);
            }

            ExtractMapMarks();
            ExtractPortals();
            ExtractTileSets();
            ExtractObjSets();
            ExtractBackgroundSets();

            // String.wz
            //List<string> stringWzFiles = wzFileManager_.GetWzFileNameListFromBase("string");
            //foreach (string stringWzFileName in stringWzFiles)
            //{
            //    wzFileManager_.LoadWzFile(stringWzFileName, version);
            //}
            //ExtractStringWzMaps();

            //// Mob WZ
            //List<string> mobWzFiles = wzFileManager_.GetWzFileNameListFromBase("mob");
            //foreach (string mobWZFile in mobWzFiles)
            //{
            //    wzFileManager_.LoadWzFile(mobWZFile, version);
            //}
            //ExtractMobFile();


            //// Load Npc
            //List<string> npcWzFiles = wzFileManager_.GetWzFileNameListFromBase("npc");
            //foreach (string npc in npcWzFiles)
            //{
            //    wzFileManager_.LoadWzFile(npc, version);
            //}
            //ExtractNpcFile();

            //// Load reactor
            //List<string> reactorWzFiles = wzFileManager_.GetWzFileNameListFromBase("reactor");
            //foreach (string reactor in reactorWzFiles)
            //{
            //    wzFileManager_.LoadWzFile(reactor, version);
            //}
            //ExtractReactorFile();

            //// Load sound
            //List<string> soundWzFiles = wzFileManager_.GetWzFileNameListFromBase("sound");
            //foreach (string soundWzFileName in soundWzFiles)
            //{
            //    wzFileManager_.LoadWzFile(soundWzFileName, version);
            //    ExtractSoundFile();
            //}


            //// Load maps
            //List<string> mapWzFiles = wzFileManager_.GetWzFileNameListFromBase("map");
            //foreach (string mapWzFileName in mapWzFiles)
            //{
            //    wzFileManager_.LoadWzFile(mapWzFileName, version);
            //}
            //for (int i_map = 0; i_map <= 9; i_map++)
            //{
            //    List<string> map_iWzFiles = wzFileManager_.GetWzFileNameListFromBase("map\\map\\map" + i_map);
            //    foreach (string map_iWzFileName in map_iWzFiles)
            //    {
            //        wzFileManager_.LoadWzFile(map_iWzFileName, version);
            //    }
            //}
            //List<string> tileWzFiles = wzFileManager_.GetWzFileNameListFromBase("map\\tile"); // this doesnt exist before 64-bit client, and is kept in Map.wz
            //foreach (string tileWzFileNames in tileWzFiles)
            //{
            //    wzFileManager_.LoadWzFile(tileWzFileNames, version);
            //}
            //List<string> objWzFiles = wzFileManager_.GetWzFileNameListFromBase("map\\obj"); // this doesnt exist before 64-bit client, and is kept in Map.wz
            //foreach (string objWzFileName in objWzFiles)
            //{
            //    wzFileManager_.LoadWzFile(objWzFileName, version);
            //}
            //List<string> backWzFiles = wzFileManager_.GetWzFileNameListFromBase("map\\back"); // this doesnt exist before 64-bit client, and is kept in Map.wz
            //foreach (string backWzFileName in backWzFiles)
            //{
            //    wzFileManager_.LoadWzFile(backWzFileName, version);
            //}
            //ExtractMapMarks();
            //ExtractPortals();
            //ExtractTileSets();
            //ExtractObjSets();
            //ExtractBackgroundSets();


            //// UI.wz
            //List<string> uiWzFiles = wzFileManager_.GetWzFileNameListFromBase("ui");
            //foreach (string uiWzFileNames in uiWzFiles)
            //{
            //    wzFileManager_.LoadWzFile(uiWzFileNames, version);
            //}

            // there are other .wz files not extracted
            // i.e Quest Character
            Logger.Report(LOGGER_LEVEL.LOGGER_LEVEL_INFO, "there are other .wz files not extracted ...");
            Logger.Report(LOGGER_LEVEL.LOGGER_LEVEL_INFO, ".wz fille are all loaded");
            return true;
        }

        private List<string> GetWzBaseNamesFromXml()
        {
            List<string> list = new List<string> { };
            const string xmlPath = wzMapleDictionary_ + wzInfoXmlName_ + ".xml";
            if(!File.Exists(xmlPath))
            {
                Logger.Report(LOGGER_LEVEL.LOGGER_LEVEL_ERROR, "xml info file " + xmlPath + " not exist");
                return list;
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlPath);
            XmlNodeList nodes = xmlDocument.SelectSingleNode("files").ChildNodes;

            foreach(XmlNode node in nodes)
            {
                var name = ((XmlElement)node).GetAttribute("name");
                var child = node.SelectSingleNode("exist");
                if (child != null && child.InnerText.Equals("true"))
                {
                    list.Add(name);
                    Logger.Report(LOGGER_LEVEL.LOGGER_LEVEL_DEBUG, "load " + name + ".wz string");
                }
            }
            return list;
        }

        #region Extractor
        /// <summary>
        /// 
        /// </summary>
        public void ExtractMobFile()
        {
            // Mob.wz
            List<WzDirectory> mobWzDirs = wzFileManager_.GetWzDirectoriesFromBase("mob");

            foreach (WzDirectory mobWzDir in mobWzDirs)
            {
            }

            // String.wz
            List<WzDirectory> stringWzDirs = wzFileManager_.GetWzDirectoriesFromBase("string");
            foreach (WzDirectory stringWzDir in stringWzDirs)
            {
                WzImage mobStringImage = (WzImage)stringWzDir?["mob.img"];
                if (mobStringImage == null)
                    continue; // not in this wz

                if (!mobStringImage.Parsed)
                    mobStringImage.ParseImage();
                foreach (WzSubProperty mob in mobStringImage.WzProperties)
                {
                    WzStringProperty nameProp = (WzStringProperty)mob["name"];
                    string name = nameProp == null ? "" : nameProp.Value;

                    wzInfoManager_.Mobs.Add(WzInfoUtil.AddLeadingZeros(mob.Name, 7), name);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExtractNpcFile()
        {
            // Npc.wz
            List<WzDirectory> npcWzDirs = wzFileManager_.GetWzDirectoriesFromBase("npc");

            foreach (WzDirectory npcWzDir in npcWzDirs)
            {
            }

            // String.wz
            List<WzDirectory> stringWzDirs = wzFileManager_.GetWzDirectoriesFromBase("string");
            foreach (WzDirectory stringWzDir in stringWzDirs)
            {
                WzImage npcImage = (WzImage)stringWzDir?["Npc.img"];
                if (npcImage == null)
                    continue; // not in this wz

                if (!npcImage.Parsed)
                    npcImage.ParseImage();
                foreach (WzSubProperty npc in npcImage.WzProperties)
                {
                    WzStringProperty nameProp = (WzStringProperty)npc["name"];
                    string name = nameProp == null ? "" : nameProp.Value;

                    wzInfoManager_.NPCs.Add(WzInfoUtil.AddLeadingZeros(npc.Name, 7), name);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExtractReactorFile()
        {
            List<WzDirectory> reactorWzDirs = wzFileManager_.GetWzDirectoriesFromBase("reactor");
            foreach (WzDirectory reactorWzDir in reactorWzDirs)
            {
                foreach (WzImage reactorImage in reactorWzDir.WzImages)
                {
                    //ReactorInfo reactor = ReactorInfo.Load(reactorImage);
                    //wzInfoManager_.Reactors[reactor.ID] = reactor;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExtractSoundFile()
        {
            List<WzDirectory> soundWzDirs = wzFileManager_.GetWzDirectoriesFromBase("sound");
            foreach (WzDirectory soundWzDir in soundWzDirs)
            {
                foreach (WzImage soundImage in soundWzDir.WzImages)
                {
                    if (!soundImage.Name.ToLower().Contains("bgm"))
                        continue;
                    if (!soundImage.Parsed)
                        soundImage.ParseImage();
                    try
                    {
                        foreach (WzImageProperty bgmImage in soundImage.WzProperties)
                        {
                            WzBinaryProperty binProperty = null;
                            if (bgmImage is WzBinaryProperty bgm)
                            {
                                binProperty = bgm;
                            }
                            else if (bgmImage is WzUOLProperty uolBGM) // is UOL property
                            {
                                WzObject linkVal = ((WzUOLProperty)bgmImage).LinkValue;
                                if (linkVal is WzBinaryProperty linkCanvas)
                                {
                                    binProperty = linkCanvas;
                                }
                            }

                            if (binProperty != null)
                                wzInfoManager_.BGMs[WzInfoUtil.RemoveExtension(soundImage.Name) + @"/" + binProperty.Name] = binProperty;
                        }
                    }
                    catch (Exception e)
                    {
                        string error = string.Format("[ExtractSoundFile] Error parsing {0}, {1} file.\r\nError: {2}", soundWzDir.Name, soundImage.Name, e.ToString());
                        Logger.Report(LOGGER_LEVEL.LOGGER_LEVEL_ERROR, error);
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExtractMapMarks()
        {
            WzImage mapWzImg = (WzImage)wzFileManager_.FindWzImageByName("map", "MapHelper.img");
            if (mapWzImg == null)
                throw new Exception("MapHelper.img not found in map.wz.");

            foreach (WzCanvasProperty mark in mapWzImg["mark"].WzProperties)
            {
                wzInfoManager_.MapMarks[mark.Name] = mark.GetLinkedWzCanvasBitmap();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExtractTileSets()
        {
            bool bLoadedInMap = false;

            WzDirectory mapWzDirs = (WzDirectory)wzFileManager_.FindWzImageByName("map", "Tile");
            if (mapWzDirs != null)
            {
                foreach (WzImage tileset in mapWzDirs.WzImages)
                    wzInfoManager_.TileSets[WzInfoUtil.RemoveExtension(tileset.Name)] = tileset;

                bLoadedInMap = true;
                return; // only needs to be loaded once
            }

            // Not loaded, try to find it in "tile.wz"
            // on 64-bit client it is stored in a different file apart from map
            if (!bLoadedInMap)
            {
                List<WzDirectory> tileWzDirs = wzFileManager_.GetWzDirectoriesFromBase("map\\tile");
                foreach (WzDirectory tileWzDir in tileWzDirs)
                {
                    foreach (WzImage tileset in tileWzDir.WzImages)
                        wzInfoManager_.TileSets[WzInfoUtil.RemoveExtension(tileset.Name)] = tileset;
                }
            }
        }

        /// <summary>
        /// Handle various scenarios ie Map001.wz exists but may only contain Back or only Obj etc
        /// </summary>
        public void ExtractObjSets()
        {
            bool bLoadedInMap = false;

            WzDirectory mapWzDirs = (WzDirectory)wzFileManager_.FindWzImageByName("map", "Obj");
            if (mapWzDirs != null)
            {
                foreach (WzImage objset in mapWzDirs.WzImages)
                    wzInfoManager_.ObjectSets[WzInfoUtil.RemoveExtension(objset.Name)] = objset;

                bLoadedInMap = true;
                return; // only needs to be loaded once
            }

            // Not loaded, try to find it in "tile.wz"
            // on 64-bit client it is stored in a different file apart from map
            if (!bLoadedInMap)
            {
                List<WzDirectory> objWzDirs = wzFileManager_.GetWzDirectoriesFromBase("map\\obj");
                foreach (WzDirectory objWzDir in objWzDirs)
                {
                    foreach (WzImage objset in objWzDir.WzImages)
                        wzInfoManager_.ObjectSets[WzInfoUtil.RemoveExtension(objset.Name)] = objset;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExtractBackgroundSets()
        {
            bool bLoadedInMap = false;

            WzDirectory mapWzDirs = (WzDirectory)wzFileManager_.FindWzImageByName("map", "Back");
            if (mapWzDirs != null)
            {
                foreach (WzImage bgset in mapWzDirs.WzImages)
                    wzInfoManager_.BackgroundSets[WzInfoUtil.RemoveExtension(bgset.Name)] = bgset;

                bLoadedInMap = true;
            }

            // Not loaded, try to find it in "tile.wz"
            // on 64-bit client it is stored in a different file apart from map
            if (!bLoadedInMap)
            {
                List<WzDirectory> backWzDirs = wzFileManager_.GetWzDirectoriesFromBase("map\\back");
                foreach (WzDirectory backWzDir in backWzDirs)
                {
                    foreach (WzImage bgset in backWzDir.WzImages)
                        wzInfoManager_.BackgroundSets[WzInfoUtil.RemoveExtension(bgset.Name)] = bgset;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ExtractStringWzMaps()
        {
            WzImage stringWzImg = (WzImage)wzFileManager_.FindWzImageByName("string", "Map.img");

            if (!stringWzImg.Parsed)
                stringWzImg.ParseImage();
            foreach (WzSubProperty mapCat in stringWzImg.WzProperties)
            {
                foreach (WzSubProperty map in mapCat.WzProperties)
                {
                    WzStringProperty streetName = (WzStringProperty)map["streetName"];
                    WzStringProperty mapName = (WzStringProperty)map["mapName"];
                    string id;
                    if (map.Name.Length == 9)
                        id = map.Name;
                    else
                        id = WzInfoUtil.AddLeadingZeros(map.Name, 9);

                    if (mapName == null)
                        wzInfoManager_.Maps[id] = new Tuple<string, string>("", "");
                    else
                        wzInfoManager_.Maps[id] = new Tuple<string, string>(streetName?.Value == null ? string.Empty : streetName.Value, mapName.Value);
                }
            }
        }

        private void ExtractPortals()
        {
            Logger.Report(LOGGER_LEVEL.LOGGER_LEVEL_INFO, "portal class not exist");
            //WzImage mapImg = (WzImage)wzFileManager_.FindWzImageByName("map", "MapHelper.img");
            //if (mapImg == null)
            //    throw new Exception("Couldnt extract portals. MapHelper.img not found.");

            //WzSubProperty portalParent = (WzSubProperty)mapImg["portal"];
            //WzSubProperty editorParent = (WzSubProperty)portalParent["editor"];
            //for (int i = 0; i < editorParent.WzProperties.Count; i++)
            //{
            //    WzCanvasProperty portal = (WzCanvasProperty)editorParent.WzProperties[i];
            //    wzInfoManager_.PortalTypeById.Add(portal.Name);
            //    PortalInfo.Load(portal);
            //}

            //WzSubProperty gameParent = (WzSubProperty)portalParent["game"]["pv"];
            //foreach (WzImageProperty portal in gameParent.WzProperties)
            //{
            //    if (portal.WzProperties[0] is WzSubProperty)
            //    {
            //        Dictionary<string, Bitmap> images = new Dictionary<string, Bitmap>();
            //        Bitmap defaultImage = null;
            //        foreach (WzSubProperty image in portal.WzProperties)
            //        {
            //            //WzSubProperty portalContinue = (WzSubProperty)image["portalContinue"];
            //            //if (portalContinue == null) continue;
            //            Bitmap portalImage = image["0"].GetBitmap();
            //            if (image.Name == "default")
            //                defaultImage = portalImage;
            //            else
            //                images.Add(image.Name, portalImage);
            //        }
            //        wzInfoManager_.GamePortals.Add(portal.Name, new PortalGameImageInfo(defaultImage, images));
            //    }
            //    else if (portal.WzProperties[0] is WzCanvasProperty)
            //    {
            //        Dictionary<string, Bitmap> images = new Dictionary<string, Bitmap>();
            //        Bitmap defaultImage = null;
            //        try
            //        {
            //            foreach (WzCanvasProperty image in portal.WzProperties)
            //            {
            //                //WzSubProperty portalContinue = (WzSubProperty)image["portalContinue"];
            //                //if (portalContinue == null) continue;
            //                Bitmap portalImage = image.GetLinkedWzCanvasBitmap();
            //                defaultImage = portalImage;
            //                images.Add(image.Name, portalImage);
            //            }
            //            wzInfoManager_.GamePortals.Add(portal.Name, new PortalGameImageInfo(defaultImage, images));
            //        }
            //        catch (InvalidCastException)
            //        {
            //            continue;
            //        } //nexon likes to toss ints in here zType etc
            //    }
            //}

            //for (int i = 0; i < wzInfoManager_.PortalTypeById.Count; i++)
            //{
            //    wzInfoManager_.PortalIdByType[wzInfoManager_.PortalTypeById[i]] = i;
            //}
            
        }

        #endregion
    }
}
