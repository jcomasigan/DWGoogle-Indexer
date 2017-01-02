using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
using System.IO;

namespace DWGoogle
{
    class ReadDwg
    {
        public static void ParseDwg(string dwgPath)
        {
            Database db = new Database(false, false);
            using (db)
            {
                try
                {
                    db.ReadDwgFile(dwgPath, FileShare.Read, false, "");
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        //iterate through each layout
                        DBDictionary layoutDic = tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead, false) as DBDictionary;
                        foreach (DBDictionaryEntry entry in layoutDic)
                        {
                            ObjectId layoutId = entry.Value;
                            Layout layout = tr.GetObject(layoutId, OpenMode.ForRead) as Layout;
                            ObjectId psId = layout.BlockTableRecordId; //layout id
                            GetStrings(db, psId);
                        }
                    }
                }
                catch
                {

                }
            }
        }

        private static void GetDrawingDetails(Database db, ObjectId layout)
        {
            RileyDwg dwg = new RileyDwg();
            using (db)
            {
                try
                {
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        BlockTableRecord btr = tr.GetObject(layout, OpenMode.ForRead) as BlockTableRecord;
                        foreach (ObjectId entId in btr)
                        {
                            Entity ent = tr.GetObject(entId, OpenMode.ForRead) as Entity;
                            if (ent != null)
                            {
                                BlockReference br = ent as BlockReference;
                                if (br != null)
                                {
                                    BlockTableRecord bd = (BlockTableRecord)tr.GetObject(br.BlockTableRecord, OpenMode.ForRead);
                                    if (bd.Name.ToUpper() == "TITL-A3L")
                                    {
                                        foreach (ObjectId arId in br.AttributeCollection)
                                        {
                                            DBObject obj = tr.GetObject(arId, OpenMode.ForRead);
                                            AttributeReference ar = obj as AttributeReference;
                                            #region Attributes of title block
                                            if (ar != null)
                                            {
                                                if (ar.Tag.ToUpper() == "CLIENT")
                                                {
                                                    dwg.Client = ar.TextString;
                                                }
                                                if (ar.Tag.ToUpper() == "PROJECT")
                                                {
                                                    dwg.Project = ar.TextString;
                                                }
                                                if (ar.Tag.ToUpper() == "TITLE")
                                                {
                                                    dwg.Title = ar.TextString;
                                                }
                                                if (ar.Tag.ToUpper() == "NO")
                                                {
                                                    dwg.DwgNo = ar.TextString;
                                                }
                                                if (ar.Tag.ToUpper() == "CADFILE")
                                                {
                                                    dwg.FileName = ar.TextString;
                                                }
                                                if (ar.Tag.ToUpper() == "DATE")
                                                {
                                                    dwg.DateDrawn = ar.TextString;
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
            }
        }

        private static void GetStrings(Database db, ObjectId layout)
        {
            List<string> textInDwg = new List<string>();
            using (db)
            {
                try
                {
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        BlockTableRecord btr = tr.GetObject(layout, OpenMode.ForRead) as BlockTableRecord;
                        foreach(ObjectId entId in btr)
                        {
                            Entity ent = tr.GetObject(entId, OpenMode.ForRead) as Entity;
                            if(ent != null)
                            {
                                MText mtext = ent as MText;
                                if(mtext != null)
                                {
                                    textInDwg.Add(mtext.Text);    
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
            }
        }
    }

}
