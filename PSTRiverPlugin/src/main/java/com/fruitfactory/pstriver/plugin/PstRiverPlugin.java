/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

package com.fruitfactory.pstriver.plugin;

import com.fruitfactory.pstriver.river.PstRiverModule;
import com.fruitfactory.pstriver.utils.PstGlobalConst;
import org.elasticsearch.common.inject.Inject;
import org.elasticsearch.common.inject.Module;
import org.elasticsearch.plugins.AbstractPlugin;
import org.elasticsearch.river.RiversModule;

/**
 *
 * @author Yariki
 */
public class PstRiverPlugin extends AbstractPlugin{
    
    @Inject
    public PstRiverPlugin() {
    }

    @Override
    public String name() {
        return "river-pst";
    }

    @Override
    public String description() {
        return "River PST Plugin";
    }

    @Override
    public void processModule(Module module) {
            if(module instanceof RiversModule){
                ((RiversModule)module).registerRiver(PstGlobalConst.PST_PREFIX, PstRiverModule.class);
            }
    }
    
    
    
}
