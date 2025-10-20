#if !NOSCGIFT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES2UserType_SCGiftItem : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		SCGiftItem data = (SCGiftItem)obj;
        // Add your writer.Write calls here.
        writer.Write(data.version);
        writer.Write(data.id);
		writer.Write(data.startTime);
		writer.Write(data.duration);

	}
	
	public override object Read(ES2Reader reader)
	{
		SCGiftItem data = new SCGiftItem();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		SCGiftItem data = (SCGiftItem)c;
		// Add your reader.Read calls here to read the data into the object.
        int version = reader.Read<System.Int32>();
        if (version >= 1)
        {
            data.id = reader.Read<System.Int32>();
            data.startTime = reader.Read<System.Int64>();
            data.duration = reader.Read<System.Int64>();
        }
    }
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_SCGiftItem():base(typeof(SCGiftItem)){}
}
#endif
