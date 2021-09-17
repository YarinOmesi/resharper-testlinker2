@file:Suppress("EXPERIMENTAL_API_USAGE","EXPERIMENTAL_UNSIGNED_LITERALS","PackageDirectoryMismatch","UnusedImport","unused","LocalVariableName","CanBeVal","PropertyName","EnumEntryName","ClassName","ObjectPropertyName","UnnecessaryVariable","SpellCheckingInspection")
package com.jetbrains.rd.ide.model

import com.jetbrains.rd.framework.*
import com.jetbrains.rd.framework.base.*
import com.jetbrains.rd.framework.impl.*

import com.jetbrains.rd.util.lifetime.*
import com.jetbrains.rd.util.reactive.*
import com.jetbrains.rd.util.string.*
import com.jetbrains.rd.util.*
import kotlin.reflect.KClass



/**
 * #### Generated from [TestLinker2Model.kt:8]
 */
class TestLinker2Model internal constructor(
) : RdExtBase() {
    //companion
    
    companion object : ISerializersOwner {
        
        override fun registerSerializersCore(serializers: ISerializers)  {
        }
        
        
        
        
        const val serializationHash = -6093571232781165861L
        
    }
    override val serializersOwner: ISerializersOwner get() = TestLinker2Model
    override val serializationHash: Long get() = TestLinker2Model.serializationHash
    
    //fields
    //methods
    //initializer
    //secondary constructor
    //equals trait
    //hash code trait
    //pretty print
    override fun print(printer: PrettyPrinter)  {
    }
    //deepClone
    override fun deepClone(): TestLinker2Model   {
        return TestLinker2Model(
        )
    }
    //contexts
}
val Solution.testLinker2Model get() = getOrCreateExtension("testLinker2Model", ::TestLinker2Model)

