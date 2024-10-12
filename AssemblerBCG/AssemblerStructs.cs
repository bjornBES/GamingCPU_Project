using System;
using System.Collections.Generic;
using System.Text;
using static HexLibrary.HexConverter;

namespace AssemblerBCG
{
    public class AssemblerStructs : AssemblerVariabels
    {
        List<Struct> m_structs = new List<Struct>();

        public void MakeStruct(string structName)
        {
            List<StructMembers> members = new List<StructMembers>();
            int size = 0;
            m_index++;

            for (; m_index < m_src.Length; m_index++)
            {
                if (string.IsNullOrEmpty(m_src[m_index]))
                {
                    continue;
                }

                if (m_src[m_index].StartsWith(".endStruct", IgnoreCasing))
                {
                    m_structs.Add(new Struct()
                    {
                        m_Name = structName,
                        m_Size = size,
                        m_StructMembers = members
                    });
                    break;
                }
                else
                {
                    string[] line = m_src[m_index].Split(' ', 2);
                    int offset = size;
                    StructMembers structMember = new StructMembers();
                    if (line[1].StartsWith(".resb", IgnoreCasing))
                    {
                        structMember.m_Size = 1;
                    }
                    else if (line[1].StartsWith(".resw", IgnoreCasing))
                    {
                        structMember.m_Size = 2;
                    }
                    else if (line[1].StartsWith(".rest", IgnoreCasing))
                    {
                        structMember.m_Size = 3;
                    }
                    else if (line[1].StartsWith(".resd", IgnoreCasing))
                    {
                        structMember.m_Size = 4;
                    }
                    else if (line[1].StartsWith(".resq", IgnoreCasing))
                    {
                        structMember.m_Size = 8;
                    }

                    structMember.m_Offset = offset;
                    structMember.m_Name = line[0].TrimEnd(':');
                    size += structMember.m_Size;
                    members.Add(structMember);
                }
            }

            m_index++;
        }
        
        public void DeclareStruct(string structName, string lableName)
        {
            if (GetStruct(structName, out Struct result))
            {
                for (int i = 0; i < result.m_StructMembers.Count; i++)
                {
                    string name = $"{lableName}_{result.m_StructMembers[i].m_Name}";
                    AddLine($"_REF_ LABEL {name} {m_file}");
                    AddLine($"_RES_ {ToHexString(result.m_StructMembers[i].m_Size)}");

                    m_Label.Add(new Label()
                    {
                        m_file = m_file,
                        m_Name = name,
                    });
                }
            }
        }

        public bool GetStruct(string structName, out Struct _out)
        {
            for (int i = 0; i < m_structs.Count; i++)
            {
                if (m_structs[i].m_Name == structName)
                {
                    _out = m_structs[i];
                    return true;
                }
            }

            _out = null;
            return false;
        }
    }
}
