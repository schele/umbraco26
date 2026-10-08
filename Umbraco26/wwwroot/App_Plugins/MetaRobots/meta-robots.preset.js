import { META_ROBOTS_DEFAULT } from './meta-robots.values.js';

/**
 * Property Value Preset for the Meta Robots editor: new pages start out as ALL.
 */
export class MetaRobotsPreset {
    async processValue(value) {
        return value ?? META_ROBOTS_DEFAULT;
    }

    destroy() {}
}

export default MetaRobotsPreset;
